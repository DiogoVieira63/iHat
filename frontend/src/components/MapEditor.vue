<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import type { Ref } from 'vue'
import { useDisplay } from 'vuetify'
import { parse, type ElementNode } from 'svg-parser'
import SvgDraw from './SvgDraw.vue'
import SvgTooltip from './SvgTooltip.vue'
import type { Option } from './SvgTooltip.vue'
import type { PropType } from 'vue'
import type { Capacete } from '@/views/SimulatorView.vue'

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
    },
    options: {
        type: String,
        required: true
    },
    capacetesPosition: {
        type: Array as PropType<Array<Capacete>>,
        required: false
    },
    capaceteSelected: {
        type: Number,
        required: false
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

const simulador = defineEmits(['addCapacete', 'selectCapacete','update::capacete'])


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
const selectedPoint = ref<number>(0)
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

const isAddingCapacete = computed(() => {
    return toggle.value === 'addCapacete'
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
    if (newValue) toggle.value = newValue
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
                selectedPoint.value = 0
            }
            toggle.value = undefined
            break
        case 'addCapacete':
            break
        case undefined:
            toggle.value = undefined
            clearPoints()
            break
    }
}

const isZoneSelected = (id: string) => {
    return selectedZone.value == id
}

const unselectZones = () => {
    selectedZone.value = null
    drawPoints.value = []
}

const selectZone = (id: string) => {
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
    const x = e.offsetX
    const y = e.offsetY
    if (isAddingCapacete.value) {
        const key = Date.now()
        simulador('addCapacete', {position: { x: x, y: y} , key: key})
        simulador('selectCapacete', key)
        return
    }else if (props.capaceteSelected != null){
        simulador('update::capacete', {position: { x: x, y: y} , key: props.capaceteSelected})
        return
    }

    if (drag.value) {
        drag.value = false
        return
    }
    if (isDrawing.value) {
        createPoint(drawPoints, x, y)
    }
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
const addPointToDrawPoints= (index : number, point : Point) => {
    drawPoints.value.splice(index, 0, point)
}

const canDelete = computed(() => {
    return !(selectedZone.value != null)
})

const canUndo = computed(() => {
    return !(isDrawing.value && drawPoints.value.length > 0)
})

const canRemove = computed(() => {
    return !(selectedPoint.value != null && drawPoints.value.length > 3)
})

const optionsEdit : Array<Option> = [
    {value: 'startDraw', text: 'Start Polygon', icon: 'mdi-shape-polygon-plus'},
    {value: 'deleteZone', text: 'Delete Zone', icon: 'mdi-delete', disabled: canDelete},
    {value: 'undo', text: 'Remove last point', icon: 'mdi-undo-variant', disabled: canUndo},
    {value: 'clear', text: 'Clear', icon: 'mdi-broom'},
    {value: 'remove', text: 'Remove Point', icon: 'mdi-close', disabled: canRemove}
] 

const optionsSimulador : Array<Option> = [
    {value: 'addCapacete', text: 'Adicionar Capacete', icon: 'mdi-plus'},
    {value: 'clear', text: 'Clear', icon: 'mdi-broom'},
    {value: 'remove', text: 'Remove Point', icon: 'mdi-close'}
] 


const optionsTooltip = computed(() => {
    if (props.options === 'Edit') return optionsEdit
    else return optionsSimulador
})


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
                    @mouseenter="(() => isDrawing || isAddingCapacete ? changeCursor('crosshair') : undefined)"
                    @mouseover="moveDrag"
                    @mouseleave="svgLeave"
                    @mousemove="showPosMouse"
                    id="my-svg"
                    :viewBox="viewBox"
                    :width="svgWidth"
                    :height="svgHeight"
                    v-bind:style="{ cursor: cursorType }"
                    v-resize="onResize"
                    class="border"
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
                    <svg-draw
                        v-if="props.edit"
                        :mousePos="mousePos"
                        :isDrawing = "isDrawing"
                        @changeCursor="changeCursor"
                        @end-drawing="endDrawing"
                        :drawPoints="drawPoints"
                        @add:drawPoints="addPointToDrawPoints"
                        :selectedPoint="selectedPoint"
                        @update:selectedPoint="selectedPoint = $event"
                        :dragPoint="dragPoint"
                        @update:dragPoint="dragPoint = $event"
                        :isDrag="drag"
                        @update:isDrag="drag = $event"
                    />
                    <image 
                        v-for=" {position,key} in props.capacetesPosition"
                        :key="key"
                        @click="simulador('selectCapacete', key)"
                        :x="position['x'] -15" :y="position['y'] -15" width="30" height="30" 
                        :href="capaceteSelected == key ?  'helmet_selected.svg' : 'helmet.svg'" />

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
        <svg-tooltip
            class="mt-5"
            v-if="props.edit"
            :toggle="toggle"
            :options="optionsTooltip"
            @update:model-value="updateEditButton" 
        />
    </v-container>
</template>

<style scoped>
/* Add your component-specific styles here */
</style>
