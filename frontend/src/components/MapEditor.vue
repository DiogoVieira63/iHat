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
import type { Zone , Point } from '@/interfaces'
import { watch } from 'vue'

const props = defineProps({
    svg: {
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
        type: Array as PropType<Array<number>>,
        required: false
    },
    zones: {
        type: Array as PropType<Array<Zone>>,
        required: true
    }
})

// if edit change, clear selected
watch(() => props.edit, (newValue) => {
    if (!newValue) {
        unselectZones()
    }
})


const baseWidth = ref(0)
const baseHeight = ref(0)
const svgWidth = ref(0)
const svgHeight = ref(0)
const scaleSVG = ref(1)
const { height, width, mdAndDown } = useDisplay()
const cursorType = ref('default')

const emit = defineEmits([
    'update:svg', 
    'update:zones',
    'addCapacete',
    'selectCapacete',
    'update::capacete',
    'selectAll',
    'unselectAll'
])

const changeCursor = (value: string) => {
    if (props.edit) cursorType.value = value
    else cursorType.value = 'default'
}

const decodeBase64 = (svg: string) => {
    // return atob(svg.split(',')[1])
    return atob(svg)
}

const resizeSVG = () => {
    let coef = 1
    const ratio = baseWidth.value / baseHeight.value
    if (baseWidth.value > baseHeight.value) {
        if (mdAndDown.value) coef = 0.90
        else coef = 0.45
        svgWidth.value = width.value * coef
        svgHeight.value = svgWidth.value / ratio
        scaleSVG.value = baseWidth.value / svgWidth.value
    } else {
        coef = 0.65
        svgHeight.value = height.value * coef
        svgWidth.value = svgHeight.value * ratio
        scaleSVG.value = baseHeight.value / svgHeight.value
    }
}


onMounted(async () => {
    const parsed = parse(decodeBase64(props.svg))
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
    baseWidth.value = w
    baseHeight.value = h
    resizeSVG()
})

const drawPoints = ref<Array<Point>>([])
const selectedZone = ref<number | null>(null)
const selectedPoint = ref<number>(0)
const toggle = ref<string | undefined>(undefined)
const drag = ref(false)
const dragPoint = ref<Point>({ x: 0, y: 0 })

const pointScale = (point : Point) => {
    return {
        x: point.x * scaleSVG.value,
        y: point.y * scaleSVG.value
    }
}


const transform = (value: number | null | undefined) => {
    if (value == null) return ''
    return `scale(${1/value})`
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

const polygonToString = (points: Array<Point>) => {
    return points.map((point) => `${point.x},${point.y}`).join(' ')
}

const createPoint = (array: Ref<Array<Point>>, x: number, y: number) => {
    const point = pointScale({ x, y })
    array.value.push(point)
}

const createPolygon = (points: Array<Point>) => {
    if (points.length >= 3) {
        let zone: Zone = {
            id: Date.now(),
            points: points.map((point) => {
                return {
                    x: point.x,
                    y: point.y
                }
            })
        }
        emit('update:zones', [...props.zones, zone])
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
        let zones = [...props.zones]
        let index = zones.findIndex((zone) => zone.id == selectedZone.value)
        zones.splice(index, 1)
        drawPoints.value = []
        emit('update:zones', zones)
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
            selectedZone.value = null
            toggle.value = undefined
            break
        case 'undo':
            undo()
            toggle.value = 'startDraw'
            break
        case 'remove':
            if (selectedPoint.value != null) {
                drawPoints.value.splice(selectedPoint.value, 1)
                selectedPoint.value = 0
            }
            toggle.value = undefined
            break
        case 'addCapacete':
            break
        case 'selectAll':
            emit('selectAll')
            toggle.value = undefined
            break
        case 'unselectAll':
            emit('unselectAll')
            toggle.value = undefined
            break
        case undefined:
            toggle.value = undefined
            clearPoints()
            break
    }
}


const isZoneSelected = (id: number) => {
    return selectedZone.value == id
}

const unselectZones = () => {
    selectedZone.value = null
    drawPoints.value = []
}

const selectZone = (id: number) => {
    if (!props.edit || isDrawing.value ) return
    unselectZones()
    selectedZone.value = id
    drawPoints.value = props.zones.find((zone) => zone.id == id)?.points || []
}

const clearPoints = () => {
    drawPoints.value = []
}

const mousePos = ref({ x: 0, y: 0 })

const showPosMouse = (e: MouseEvent) => {
    mousePos.value = pointScale({ x: e.offsetX, y: e.offsetY })
}

const svgClick = (e: MouseEvent) => {
    const x = e.offsetX
    const y = e.offsetY
    if (isAddingCapacete.value) {
        const key = Date.now()
        emit('addCapacete', { position: { x: x, y: y }, key: key })
        emit('selectCapacete', key)
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
        dragPoint.value.x = e.offsetX * scaleSVG.value
        dragPoint.value.y = e.offsetY * scaleSVG.value 
    }
}

const svgLeave = () => {
    changeCursor('default')
    drag.value = false
}

const addPointToDrawPoints = (index: number, point: Point) => {
    drawPoints.value.splice(index, 0, point)
}

const canDelete = computed(() => {
    return !(selectedZone.value != null)
})
const polygonStrokeArray = (id: number) => {
    if (isZoneSelected(id)) return `${5 * scaleSVG.value} ${10  * scaleSVG.value}`
    else return '0'
}

const createBlobURL = (svg: string) => {
  // Decode the base64 SVG content
  const decodedSVG = atob(svg);
  
  // Convert decoded SVG content to a Blob
  const blob = new Blob([decodedSVG], { type: 'image/svg+xml' });
  
  // Create a URL for the Blob
  return URL.createObjectURL(blob);
}

const canUndo = computed(() => {
    return !(isDrawing.value && drawPoints.value.length > 0)
})

const canRemove = computed(() => {
    return !(selectedPoint.value != null && drawPoints.value.length > 3)
})

const optionsEdit: Array<Option> = [
    { value: 'startDraw', text: 'Start Polygon', icon: 'mdi-shape-polygon-plus' },
    { value: 'deleteZone', text: 'Delete Zone', icon: 'mdi-delete', disabled: canDelete },
    { value: 'undo', text: 'Remove last point', icon: 'mdi-undo-variant', disabled: canUndo },
    { value: 'clear', text: 'Clear', icon: 'mdi-broom' },
    { value: 'remove', text: 'Remove Point', icon: 'mdi-close', disabled: canRemove }
]

const optionsSimulador: Array<Option> = [
    { value: 'addCapacete', text: 'Adicionar Capacete', icon: 'mdi-plus' },
    { value: 'selectAll', text: 'Select All', icon: 'mdi-select' },
    { value: 'unselectAll', text: 'Unselect All', icon: 'mdi-select-off' }
]

const optionsTooltip = computed(() => {
    if (props.options === 'Edit') return optionsEdit
    else return optionsSimulador
})
</script>

<template>
    <v-container v-if="active">
        <v-sheet height="65vh" class="d-flex align-center">
            <v-row justify="center">
                <svg
                    @click="svgClick"
                    @mouseenter="() => (isDrawing || isAddingCapacete ? changeCursor('crosshair') : undefined)"
                    @mouseleave="svgLeave"
                    @mousemove="showPosMouse"
                    id="my-svg"
                    :viewBox="viewBox"
                    :width="svgWidth"
                    :height="svgHeight"
                    v-bind:style="{ cursor: cursorType }"
                    class="border"
                    v-resize="resizeSVG"
                    @mouseover="moveDrag"
                >
                    <!--image :href="props.svg" :width="svgWidth" :height="svgHeight" /-->
                    <image
                        :xlink:href="createBlobURL(props.svg)   "
                        :width="svgWidth"
                        :height="svgHeight"
                    />

                    <polygon
                        v-for="{ points, id } in props.zones"
                        :id="id.toString()"
                        :points="polygonToString(points)"
                        fill="red"
                        fill-opacity="0.5"
                        @click="selectZone(id)"
                        :transform="transform(scaleSVG)"
                        stroke="black"
                        :stroke-width="3 * scaleSVG"
                        :stroke-dasharray="polygonStrokeArray(id)"
                        :key="id"
                    />
                    <svg-draw
                        v-if="props.edit"
                        :mousePos="mousePos"
                        :isDrawing="isDrawing"
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
                        v-for="{ position, key } in props.capacetesPosition"
                        :key="key"
                        @click="emit('selectCapacete', key)"
                        :x="position['x'] - 15"
                        :y="position['y'] - 15"
                        width="30"
                        height="30"
                        :href="
                            props.capaceteSelected?.includes(key)
                                ? '/helmet_selected.svg'
                                : '/helmet.svg'
                        "
                    />
                </svg>
            </v-row>
        </v-sheet>
        <template class="d-flex justify-center">
            <v-sheet
                v-if="isDrawing"
                class="my-10 d-flex justify-center border pa-3"
                width="500px"
                rounded="xl"
            >
                <v-icon
                    color="info"
                    class="mr-2"
                    >mdi-information</v-icon
                >
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