<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import type { Ref } from 'vue'
import { useDisplay } from 'vuetify'
import { parse, type ElementNode } from 'svg-parser'
let id = 0

const props = defineProps({
    svgSrc: {
        type: String,
        required: true
    },
    edit: {
        type: Boolean,
        required: true
    },
    active: {
        type: Boolean,
        required: true
    }
})

interface Point {
    x: number
    y: number
    coef?: number
}

interface Polygon {
    points: string
    pointsArray: Array<Point>
    id: number
    stroke: string
    multiplier: number
    coef?: number
}

const coef = ref(1)
const baseWidth = ref(0)
const baseHeight = ref(0)
const ratio = ref(1)
const svgWidth = ref(0)
const svgHeight = ref(0)
const { width, mdAndDown } = useDisplay()
const cursorType = ref('default')


const changeCursor = (value: string) => {
    if (props.edit) cursorType.value = value
    else cursorType.value = 'default'
}


// Scale the max to 900px
const scale = (x: number, y: number) => {
    const max = width.value * 0.3
    const ratio = x / y
    if (x > y) {
        return [max, max / ratio]
    } else {
        return [max * ratio, max]
    }
}

onMounted(async () => {
    const response = await fetch(props.svgSrc)
    const data = await response.text()
    const parsed = parse(data)
    const node: ElementNode = parsed.children[0] as ElementNode
    const height = node.properties ? node.properties.height : 100
    const vB = node.properties ? (node.properties.viewBox as string) : '0 0 100 100'

    let values = vB.split(' ').map(Number)
    if (typeof height === 'string' && height.includes('mm')) {
        const viewBoxMM = vB.split(' ').map(Number)
        values = viewBoxMM.map((mm: number) => mm * 3.78) // Convert from mm to px
    }
    const w = values[2] - values[0]
    const h = values[3] - values[1]
    coef.value = width.value / 2 / w
    const [newW, newH] = scale(w, h)
    baseWidth.value = newW
    baseHeight.value = newH
    svgWidth.value = baseWidth.value
    svgHeight.value = baseHeight.value
    ratio.value = newW / newH
})

const coefSvg = ref<number>(1)

const polygons = ref<{ [key: string]: Polygon }>({})
const drawPoints = ref<Array<Point>>([])
const selectedZone = ref<string | null>(null)
const selectedPoint = ref<number | null>(null)
const toggle = ref<string | undefined>(undefined)

const drag = ref(false)
const dragPoint = ref<Point>({x: 0, y: 0})


const onResize = () => {
    let coefX = 1
    if (mdAndDown.value) {
        coefX = (width.value * 0.9) / baseWidth.value
    } else {
        coefX = (width.value * 0.5) / baseWidth.value
    }
    svgWidth.value = coefX > 1 ? baseWidth.value : baseWidth.value * coefX
    svgHeight.value = svgWidth.value / ratio.value
    coefSvg.value = svgWidth.value / baseWidth.value
}


function transform(value: number | null | undefined) {
    if (!value) {
        return `scale(${coefSvg.value})`
    }
    let res = coefSvg.value / value
    return `scale(${res})`
}



const isDrawing = computed(() => {
    return toggle.value === 'startDraw'
})

const viewBox = computed(() => {
    return `0 0 ${svgWidth.value} ${svgHeight.value}`
})


const polygonToString = (index : number) => {
    let points = polygons.value[index].pointsArray
    let multiplier = 1
    return points.map((point) => `${point.x * multiplier},${point.y * multiplier}`).join(' ')
}

const createPoint = (array: Ref<Array<Point>>, x: number, y: number) => {
    const point: Point = {
        x: x,
        y: y,
        coef: coefSvg.value
    }
    array.value.push(point)
}

const createPolygon = (points: Array<Point>) => {
    if (points.length >= 3) {
        let multiplier = 1
        if (coefSvg.value < 1) {
            multiplier = 1 / coefSvg.value
        }
        const pointString = points
            .map((point) => `${point.x * multiplier},${point.y * multiplier}`)
            .join(' ')
        let polygon: Polygon = {
            points: pointString,
            pointsArray: points,
            multiplier: multiplier,
            id: id,
            stroke: 'none',
            coef: coefSvg.value * multiplier
        }
        polygons.value[id] = polygon
        id += 1
    }
}


const endDrawing = () => {
    let points = [...drawPoints.value]
    createPolygon(points)
    clearPoints()
    toggle.value = undefined
}

const undo = () => {
    if (drawPoints.value.length > 0) {
        drawPoints.value.pop()
    }
}

const deleteZone = () => {
    if (selectedZone.value) {
        delete polygons.value[selectedZone.value]
        selectedZone.value = null
        drawPoints.value = []
    }
}


const updateEditButton = (newValue: string | null) => {
    switch (newValue) {
        case 'startDraw':
            drawPoints.value = []
            unselectZones()
            break
        case 'deleteZone':
            deleteZone()
            toggle.value = undefined
            break
        case 'clear':
            drawPoints.value = []
            polygons.value = {}
            selectedZone.value = null
            toggle.value = undefined
            break
        case 'undo':
            undo()
            toggle.value = 'startDraw'
            break
        case 'remove':
            if (selectedPoint.value != null){
                drawPoints.value.splice(selectedPoint.value, 1)
                selectedPoint.value = null
            }
            toggle.value = undefined
            break
        case undefined:
            clearPoints()
            break
    }
}


const lastPos = computed(() => {
    return drawPoints.value[drawPoints.value.length - 1]
})


const drawLines = computed(() => {
    if (toggle.value !== 'startDraw') return []
    let lines: Array<[Point, Point]> = []
    for (let i = 0; i < drawPoints.value.length - 1; i++) {
        let line: [Point, Point] = [drawPoints.value[i], drawPoints.value[i + 1]]
        lines.push(line)
    }
    return lines
})


const drawPointsMiddle = computed(() => {
    let points: Array<Point> = []
    for (let i = 0; i < drawPoints.value.length - 1; i++) {
        let point: Point = {
            x: (drawPoints.value[i].x + drawPoints.value[i + 1].x) / 2,
            y: (drawPoints.value[i].y + drawPoints.value[i + 1].y) / 2
        }
        points.push(point)
    }
    // last point to first point
    if (drawPoints.value.length > 2 && !isDrawing.value) {
        let point: Point = {
            x: (drawPoints.value[drawPoints.value.length - 1].x + drawPoints.value[0].x) / 2,
            y: (drawPoints.value[drawPoints.value.length - 1].y + drawPoints.value[0].y) / 2
        }
        points.push(point)
    }
    return points
})

const drawPointsAll = computed(() => {
    let all = []
    for (let i = 0; i < drawPoints.value.length; i++) {
        all.push({'point': drawPoints.value[i], 'index': i, 'real': true} )
        if (!isDrawing.value && i < drawPointsMiddle.value.length) all.push( {'point': drawPointsMiddle.value[i], 'index': i, 'real': false} )
    }
    return all
})


const isZoneSelected = (id: string) => {
    return selectedZone.value == id
}

const unselectZones = () => {
    selectedZone.value = null
    drawPoints.value = []
}

const selectZone = (id: string) => {
    if (toggle.value === 'addMark') {
        return
    }
    unselectZones()
    selectedZone.value = id
    drawPoints.value = polygons.value[id].pointsArray
    polygons.value[id].stroke = 'black'
}

const clearPoints = () => {
    drawPoints.value = []
}

const mousePos = ref({ x: 0, y: 0 })

const showPosMouse = (e: MouseEvent) => {
    mousePos.value.x = e.offsetX
    mousePos.value.y = e.offsetY
}

const svgClick = (e: MouseEvent) => {
    if (drag.value) {
        drag.value = false
        return
    }
    if (toggle.value === 'startDraw' || toggle.value === 'addMark') {
        const x = e.offsetX
        const y = e.offsetY
        if (toggle.value === 'startDraw') {
            createPoint(drawPoints, x, y)
        }
    }
}

const pointClick = (index : number, real : boolean) => {
    if(real && index == 0 && toggle.value == 'startDraw'){
        endDrawing()
    }
    else{
        drag.value = true 
        if(real){
            dragPoint.value = drawPoints.value[index]
            selectedPoint.value = index
        }
        else{
            drawPoints.value.splice(index + 1, 0, drawPointsMiddle.value[index])
            dragPoint.value = drawPoints.value[index + 1]
            selectedPoint.value = index + 1
        }
        changeCursor('grabbing')
    }
}  

const pointOver = (index : number) => {
    if (isDrawing.value && index == 0){
        changeCursor('default')
    }
    else if (!drag.value){
        changeCursor('grab')
    }
}

const pointLeave = () => {
    if (isDrawing.value) changeCursor('crosshair')
    else if (!drag.value) changeCursor('default')
}

const isPointSelected = (index : number) => {
    return selectedPoint.value == index
}


const moveDrag = (e: MouseEvent) => {
    if (drag.value) {
        dragPoint.value.x = e.offsetX
        dragPoint.value.y = e.offsetY
    }
}

const svgLeave = () => {
    changeCursor('default')
    drag.value = false
}

const polygonStrokeArray = (id: string) => {
    if (isZoneSelected(id)) return '5 10'
     else return '0'
}


</script>

<template>
    <v-container v-if="active">
        <v-sheet
            :height="mdAndDown ? svgHeight : '65vh'"
            class="d-flex align-center justify-center flex-wrap my-auto py-4"
        >
            <v-row justify="center">
                <svg
                    @click="svgClick"
                    @mouseenter="(() => isDrawing ? changeCursor('crosshair') : undefined)"
                    @mouseleave="svgLeave"
                    @mousemove="showPosMouse"
                    id="my-svg"
                    :viewBox="viewBox"
                    :width="svgWidth"
                    :height="svgHeight"
                    v-bind:style="{ cursor: cursorType }"
                    v-resize="onResize"
                    class="border"
                    @mouseover="moveDrag"
                >
                    <image :href="props.svgSrc" :width="svgWidth" :height="svgHeight" />

                    <polygon
                        v-for="(polygon, id) in polygons"
                        :id="id.toString()"
                        :points="polygonToString(Number(id))"
                        fill="red"
                        fill-opacity="0.5"
                        @click="selectZone(String(id))"
                        stroke="red"
                        stroke-width="3"
                        :stroke-dasharray="polygonStrokeArray(String(id))"
                        :transform="transform(polygon.coef)"
                        :key="id"
                    />
                    <circle
                        v-for="{point, index, real} in drawPointsAll"
                        :cx="point.x"
                        :cy="point.y"
                        :r="real ? 8 : 5"
                        fill="white"
                        :stroke="isPointSelected(index) && real? 'red' : 'black'"
                        :stroke-width="isPointSelected(index) && real? 3 : 1"
                        :transform="transform(point.coef)"
                        :key="index"
                        @mouseover="pointOver(index)"
                        @mouseleave="pointLeave()"
                        @mousedown="pointClick(index,real)"
                    />

                    <line
                        v-for="(points, index) in drawLines"
                        :x1="points[0].x"
                        :y1="points[0].y"
                        :x2="points[1].x"
                        :y2="points[1].y"
                        stroke="red"
                        stroke-width="3"
                        :key="index"
                    />
                    <line
                        v-if="isDrawing && drawPoints.length > 0"
                        :x1="lastPos.x"
                        :y1="lastPos.y"
                        :x2="mousePos.x"
                        :y2="mousePos.y"
                        stroke="red"
                        stroke-dasharray="5 10"
                        stroke-width="3"
                    />
                </svg>
            </v-row>
        </v-sheet>
        <template class="d-flex justify-center">
            <v-sheet
                v-if="isDrawing"
                class="my-5 d-flex justify-center border pa-3"
                width="500px"
                rounded="xl"
            >
                <v-icon color="info" class="mr-2">mdi-information</v-icon>
                <p>Conecte com o primeiro para terminar o polígono.</p>
            </v-sheet>
        </template>
        <v-row v-if="props.edit" justify="center">
            <v-btn-toggle
                v-model="toggle"
                color="info"
                variant="outlined"
                @update:model-value="updateEditButton"
            >
                <v-tooltip text="Start Polygon" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-shape-polygon-plus"
                            value="startDraw"
                        ></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Delete Zone" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-delete"
                            value="deleteZone"
                            :disabled="selectedZone == null"
                        ></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Remove last point" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-undo-variant"
                            value="undo"
                            :disabled="!isDrawing || drawPoints.length == 0"
                        ></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Clear" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-broom" value="clear"></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Remove Point" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-close" value="remove"
                            :disabled="selectedPoint == null || drawPoints.length <= 3"
                        ></v-btn>
                    </template>
                </v-tooltip>

            </v-btn-toggle>
        </v-row>
    </v-container>
</template>

<style scoped>
/* Add your component-specific styles here */
</style>
