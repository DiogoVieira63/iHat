<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import type { Ref } from 'vue'
import { useDisplay } from 'vuetify'
import { parse, type ElementNode } from 'svg-parser'
import type { Zone , Point } from '@/interfaces'
import type { PropType } from 'vue'
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


const emit = defineEmits(['update:svg', 'update:zones'])

const baseWidth = ref(0)
const baseHeight = ref(0)
const svgWidth = ref(0)
const svgHeight = ref(0)
const scaleSVG = ref(1)
const { height, width, mdAndDown } = useDisplay()
const cursorType = ref('default')

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
const selectedPoint = ref<number | null>(null)
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
        all.push({ point: drawPoints.value[i], index: i, real: true })
        if (!isDrawing.value && i < drawPointsMiddle.value.length)
            all.push({ point: drawPointsMiddle.value[i], index: i, real: false })
    }
    return all
})

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
    if (!props.edit) return
    if (drag.value) {
        drag.value = false
        return
    }
    if (toggle.value === 'startDraw') {
        const x = e.offsetX
        const y = e.offsetY
        if (toggle.value === 'startDraw') {
            createPoint(drawPoints, x, y)
        }
    }
}

const pointClick = (index: number, real: boolean) => {
    if (real && index == 0 && toggle.value == 'startDraw') {
        endDrawing()
    } else {
        drag.value = true
        if (real) {
            dragPoint.value = drawPoints.value[index]
            selectedPoint.value = index
        } else {
            drawPoints.value.splice(index + 1, 0, drawPointsMiddle.value[index])
            dragPoint.value = drawPoints.value[index + 1]
            selectedPoint.value = index + 1
        }
        changeCursor('grabbing')
    }
}

const pointOver = (index: number) => {
    if (isDrawing.value && index == 0) {
        changeCursor('pointer')
    } else if (!drag.value) {
        changeCursor('grab')
    }
}

const pointLeave = () => {
    if (isDrawing.value) changeCursor('crosshair')
    else if (!drag.value) changeCursor('default')
}

const isPointSelected = (index: number) => {
    return selectedPoint.value == index
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

</script>

<template>
    <v-container v-if="active">
        <v-sheet height="65vh" class="d-flex align-center">
            <v-row justify="center">
                <svg
                    @click="svgClick"
                    @mouseenter="() => (isDrawing ? changeCursor('crosshair') : undefined)"
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
                        :xlink:href="createBlobURL(props.svg)"
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
                    <circle
                        v-for="{ point, index, real } in drawPointsAll"
                        :cx="point.x / scaleSVG"
                        :cy="point.y / scaleSVG"
                        :r="real ? 8 : 5"
                        fill="white"
                        :stroke="isPointSelected(index) && real ? 'red' : 'black'"
                        :stroke-width="isPointSelected(index) && real ? 3 : 1"
                        :key="index"
                        @mouseover="pointOver(index)"
                        @mouseleave="pointLeave()"
                        @mousedown="pointClick(index, real)"
                    />

                    <line
                        v-for="(points, index) in drawLines"
                        :x1="points[0].x / scaleSVG"
                        :y1="points[0].y / scaleSVG"
                        :x2="points[1].x / scaleSVG"
                        :y2="points[1].y / scaleSVG"
                        stroke="red"
                        stroke-width="3"
                        :key="index"
                    />
                    <line
                        v-if="isDrawing && drawPoints.length > 0"
                        :x1="lastPos.x  /scaleSVG"
                        :y1="lastPos.y  /scaleSVG"
                        :x2="mousePos.x /scaleSVG"
                        :y2="mousePos.y /scaleSVG"
                        stroke="red"
                        stroke-dasharray="5 10"
                        stroke-width="3 "
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
        <v-row
            v-if="props.edit"
            justify="center"
            class="mt-5"
        >
            <v-btn-toggle
                v-model="toggle"
                color="info"
                variant="outlined"
                @update:model-value="updateEditButton"
            >
                <v-tooltip
                    text="Start Polygon"
                    location="bottom"
                >
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-shape-polygon-plus"
                            value="startDraw"
                        ></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip
                    text="Delete Zone"
                    location="bottom"
                >
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-delete"
                            value="deleteZone"
                            :disabled="selectedZone == null"
                        ></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip
                    text="Remove last point"
                    location="bottom"
                >
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-undo-variant"
                            value="undo"
                            :disabled="!isDrawing || drawPoints.length == 0"
                        ></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip
                    text="Clear"
                    location="bottom"
                >
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-broom"
                            value="clear"
                        ></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip
                    text="Remove Point"
                    location="bottom"
                >
                    <template v-slot:activator="{ props }">
                        <v-btn
                            v-bind="props"
                            icon="mdi-close"
                            value="remove"
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