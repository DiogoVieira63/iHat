<script setup lang="ts">
import { ref, onUpdated, onMounted, computed } from 'vue'
import type { Ref } from 'vue'
import { useDisplay } from 'vuetify'
import { parse, type ElementNode } from 'svg-parser';

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
    id: number
    stroke: string
    coef?: number
}

const coef = ref(1)
const baseWidth = ref(0)
const baseHeight = ref(0)
const ratio = ref(1)
const svgWidth = ref(0)
const svgHeight = ref(0)
const { width, mdAndDown } = useDisplay()

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
    const response = await fetch(props.svgSrc);
    const data = await response.text();
    const parsed = parse(data);
    const node: ElementNode = parsed.children[0] as ElementNode
    const height = node.properties ? node.properties.height : 100
    const vB = node.properties ? node.properties.viewBox as string : "0 0 100 100"

    let values = vB.split(' ').map(Number);
    if (typeof (height) === 'string' && height.includes('mm')) {
        const viewBoxMM = vB.split(' ').map(Number);
        values = viewBoxMM.map((mm: number) => mm * 3.78); // Convert from mm to px
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
});



const coefSvg = ref<number>(1)

const polygons = ref<{ [key: string]: Polygon }>({})
const drawPoints = ref<Array<Point>>([])
// drawLines - Array of tuple of points
const drawLines = ref<Array<[Point, Point]>>([])
const randomPoints = ref<Array<Point>>([])
const zones = ref<Array<Polygon>>([])
const selectedZone = ref<string | null>(null)
const toggle = ref<string| undefined>(undefined)
const intersected = ref({})

const svg = ref(null)
const polygonsDom = ref(null)

const updateEditButton = (newValue: string | null) => {
    switch (newValue) {
        case 'startDraw':
            startDrawing()
            break
        case 'deleteZone':
            deleteZone()
            toggle.value = undefined
            break
        case 'clear':
            randomPoints.value = []
            drawPoints.value = []
            polygons.value = {}
            zones.value = []
            selectedZone.value = null
            intersected.value = {}
            toggle.value = undefined
            break
        case 'undo':
            undo()
            toggle.value = "startDraw"
            break
        case undefined:
            clearPoints()
            break
    }
}

onUpdated(() => {
})

const startDrawing = () => {
    drawPoints.value = []
}

const endDrawing = () => {
    // copy drawPoints
    let points = [...drawPoints.value]
    createPolygon(points)
    clearPoints()
    toggle.value = undefined
}

const lastPos = computed(() => {
    return drawPoints.value[drawPoints.value.length - 1]
})

const svgClick = (e: MouseEvent) => {
    if (toggle.value === 'startDraw' || toggle.value === 'addMark') {
        const x = e.offsetX
        const y = e.offsetY
        if (toggle.value === 'startDraw') {
            let point: Point = { x, y }
            if (drawPoints.value.length > 0) {
                let line: [Point, Point] = [lastPos.value, point]
                drawLines.value.push(line)
            }
            createPoint(drawPoints, x, y)
        } else if (toggle.value === 'addMark') {
            createPoint(randomPoints, x, y)
            // checkIfIntersect()
        }
    }
}
const createPoint = (array: Ref<Array<Point>>, x: number, y: number) => {
    const point: Point = {
        x: x,
        y: y,
        coef: coefSvg.value
    }
    array.value.push(point)
}

const undo = () => {
    if (drawPoints.value.length > 0) {
        drawPoints.value.pop()
        if (drawPoints.value.length > 0) {
            drawLines.value.pop()
        }
    }
}


const createPolygon = (points: Array<Point>) => {
    if (points.length >= 3) {
        let multiplier = 1
        if (coefSvg.value < 1) {
            multiplier = 1 / coefSvg.value

        }
        const pointString = points.map((point) => `${point.x * multiplier},${point.y * multiplier}`).join(' ')
        let polygon: Polygon = { points: pointString, id: id, stroke: 'none', coef: coefSvg.value * multiplier }
        polygons.value[id] = polygon
        zones.value.push(polygon)
        selectZone(String(id))
        id += 1
    }
}

const unselectZones = () => {
    for (const key of Object.keys(polygons.value)) {
        polygons.value[key].stroke = 'none'
    }
}

const selectZone = (id: string) => {
    if (toggle.value === 'addMark') {
        return
    }
    unselectZones()
    selectedZone.value = id
    polygons.value[id].stroke = 'black'
}

const deleteZone = () => {
    if (selectedZone.value) {
        delete polygons.value[selectedZone.value]
        selectedZone.value = null
    }
}

const clearPoints = () => {
    drawPoints.value = []
    drawLines.value = []
}
/*
const checkIfIntersect = () => {
  // iterate to randomPoints
  let int = {}
  for (let j = 0; j < polygonsDom.value.length; j++) {
    let polygon = polygonsDom.value[j]
    int[polygon.id] = []
    for (let i = 0; i < randomPoints.value.length; i++) {
      let p = randomPoints.value[i]
      // iterate to polygons
      let point = svg.value.createSVGPoint()
      point.x = p.x
      point.y = p.y
      if (polygon.isPointInFill(point) || polygon.isPointInStroke(point)) {
        int[polygon.id].push(p)
      }
    }
  }
  intersected.value = int
}
*/

const onResize = () => {
    let coefX = 1;
    if (mdAndDown.value) {
        coefX = width.value * 0.9 / baseWidth.value
    }
    else {
        coefX = width.value * 0.5 / baseWidth.value
    }
    svgWidth.value = coefX > 1 ? baseWidth.value : baseWidth.value * coefX
    svgHeight.value = svgWidth.value / ratio.value
    coefSvg.value = svgWidth.value / baseWidth.value
}

const viewBox = computed(() => {
    return `0 0 ${svgWidth.value} ${svgHeight.value}`
})


function transform(value: number | null | undefined) {
    if (!value) {
        return `scale(${coefSvg.value})`
    }
    let res = coefSvg.value / value
    return `scale(${res})`
}

const cursorType = ref('default')

const isDrawing = computed(() => {
    return toggle.value === 'startDraw'
})


const changeCursor = (value: string) => {
    if (props.edit && toggle.value == 'startDraw') cursorType.value = value
    else cursorType.value = 'default'
}

const mousePos = ref({ x: 0, y: 0 })
const showPosMouse = (e: MouseEvent) => {
    mousePos.value.x = e.offsetX
    mousePos.value.y = e.offsetY
}



</script>

<template>
    <v-container v-if="active">
        <v-sheet :height="mdAndDown ? svgHeight : '65vh'" class="d-flex align-center justify-center flex-wrap my-auto py-4">
            <v-row justify="center">
                <svg @click="svgClick" @mouseenter="changeCursor('crosshair')" @mouseleave="changeCursor('default')" @mousemove="showPosMouse"
                    ref="svg" id="my-svg" :viewBox="viewBox" :width="svgWidth" :height="svgHeight"
                    v-bind:style="{ cursor: cursorType }" v-resize="onResize" class="border">
                    <image :href="props.svgSrc" :width="svgWidth" :height="svgHeight" />
                    <polygon v-for="(polygon, id) in polygons" ref="polygonsDom" :id="id.toString()"
                        :points="polygon.points" fill="red" fill-opacity="0.5" @click="selectZone(String(id))"
                        :stroke="polygon.stroke" :transform="transform(polygon.coef)" :key="id" />
                    <circle v-for="(point, index) in drawPoints" :cx="point.x" :cy="point.y" r="3" fill="red"
                        :transform="transform(point.coef)" :key="index" />
                    <circle v-for="(point, index) in randomPoints" :cx="point.x" :cy="point.y" r="3" fill="red"
                        :transform="transform(point.coef)" :key="index" />
                    <circle v-if="drawPoints.length > 0" :cx="drawPoints[0].x" :cy="drawPoints[0].y" r="8"  fill="white" stroke="red"
                        :transform="transform(drawPoints[0].coef)"  @click="endDrawing"  @mouseenter="changeCursor('default')" @mouseleave="changeCursor('crosshair')"/>
                    <line v-for="(points,index) in drawLines"
                        :x1="points[0].x" :y1="points[0].y" 
                        :x2="points[1].x" :y2="points[1].y" 
                        stroke="black" stroke-width="2"
                        :key="index"
                    />
                    <line v-if="drawPoints.length > 0" 
                        :x1="lastPos.x" :y1="lastPos.y" 
                        :x2="mousePos.x" :y2="mousePos.y" 
                        stroke="red" stroke-dasharray="5 10" stroke-width="3" />
                </svg>
            </v-row>
        </v-sheet>
        <template  class="d-flex justify-center" >
            <v-sheet v-if="isDrawing" class="my-5 d-flex justify-center border pa-3" width="500px" rounded="xl">
                <v-icon color="info" class="mr-2" >mdi-information</v-icon>
                <p>Conecte com o primeiro para terminar o polígono.</p>
            </v-sheet>
        </template>
        <v-row v-if="props.edit" justify="center">
            <v-btn-toggle v-model="toggle" color="info" variant="outlined" @update:model-value="updateEditButton" >
                <v-tooltip text="Start Polygon" location="bottom"  >
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-shape-polygon-plus" value="startDraw"></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Delete Zone" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-delete" value="deleteZone" :disabled="selectedZone == null"></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Remove last point" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-undo-variant" value="undo" :disabled="drawPoints.length == 0"></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Clear" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-broom" value="clear"></v-btn>
                    </template>
                </v-tooltip>
                <!--v-tooltip text="Remove" location="bottom">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-broom" value="clear"></v-btn>
                    </template>
                </v-tooltip-->
            </v-btn-toggle>
        </v-row>

    </v-container>
</template>

<style scoped>
/* Add your component-specific styles here */
</style>
