<script setup lang="ts">
import type { PropType } from 'vue';
import { computed } from 'vue';

const props = defineProps({
    drawPoints: {
        type: Array as PropType<Array<Point>>,
        required: true
    },
    mousePos: {
        type: Object as PropType<Point>,
        required: true
    },
    isDrawing: {
        type: Boolean,
        required: true
    },
    selectedPoint: {
        type: Number,
        required: true
    },
    dragPoint: {
        type: Object as PropType<Point>,
        required: true
    },
    isDrag: {
        type: Boolean,
        required: true
    },
    coefSvg: {
        type: Number,
        required: true
    }
})

interface Point {
    x: number
    y: number
    coef?: number
}

const emit = defineEmits([
    'endDrawing',
    'changeCursor',
    'add:drawPoints',
    'update:selectedPoint',
    'update:dragPoint',
    'update:isDrag'
])

const isPointSelected = (index: number) => {
    return props.selectedPoint == index
}

const drawPointsMiddle = computed(() => {
    let points: Array<Point> = []
    for (let i = 0; i < props.drawPoints.length - 1; i++) {
        let point: Point = {
            x: (props.drawPoints[i].x + props.drawPoints[i + 1].x) / 2,
            y: (props.drawPoints[i].y + props.drawPoints[i + 1].y) / 2
        }
        points.push(point)
    }
    // last point to first point
    if (props.drawPoints.length > 2) {
        let point: Point = {
            x: (props.drawPoints[props.drawPoints.length - 1].x + props.drawPoints[0].x) / 2,
            y: (props.drawPoints[props.drawPoints.length - 1].y + props.drawPoints[0].y) / 2
        }
        points.push(point)
    }
    return points
})

const drawPointsAll = computed(() => {
    let all = []
    for (let i = 0; i < props.drawPoints.length; i++) {
        all.push({ point: props.drawPoints[i], index: i, real: true })
        if (!props.isDrawing && i < drawPointsMiddle.value.length)
            all.push({ point: drawPointsMiddle.value[i], index: i, real: false })
    }
    return all
})

const drawLines = computed(() => {
    if (!props.isDrawing) return []
    let lines: Array<[Point, Point]> = []
    for (let i = 0; i < props.drawPoints.length - 1; i++) {
        let line: [Point, Point] = [props.drawPoints[i], props.drawPoints[i + 1]]
        lines.push(line)
    }
    return lines
})

const pointClick = (index: number, real: boolean) => {
    if (real && index == 0 && props.isDrawing) {
        emit('endDrawing')
    } else {
        emit('update:isDrag', true)
        if (real) {
            emit('update:dragPoint', props.drawPoints[index])
            emit('update:selectedPoint', index)
        } else {
            emit('add:drawPoints', index + 1, drawPointsMiddle.value[index])
            emit('update:dragPoint', props.drawPoints[index + 1])
            emit('update:selectedPoint', index + 1)
        }
        emit('changeCursor', 'grabbing')
    }
}

function transform() {
    let res = 1 / props.coefSvg
    return `scale(${res})`
}

const lastPos = computed(() => {
    return props.drawPoints[props.drawPoints.length - 1]
})

const pointOver = (index: number) => {
    if (props.isDrawing && index == 0) {
        emit('changeCursor', 'pointer')
    } else if (!props.isDrag) {
        emit('changeCursor', 'grab')
    }
}

const pointLeave = () => {
    if (props.isDrawing) emit('changeCursor', 'crosshair')
    else if (!props.isDrag) emit('changeCursor', 'default')
}
</script>
<template>
    <line
        v-for="(points, index) in drawLines"
        :x1="points[0].x"
        :y1="points[0].y"
        :x2="points[1].x"
        :y2="points[1].y"
        stroke="red"
        stroke-width="3"
        :key="index"
        :transform="transform()"
    />
    <line
        v-if="isDrawing && props.drawPoints.length > 0"
        :x1="lastPos.x"
        :y1="lastPos.y"
        :x2="mousePos.x"
        :y2="mousePos.y"
        stroke="red"
        stroke-dasharray="5 10"
        stroke-width="3"
        :transform="transform()"
    />
    <circle
        v-for="{ point, index, real } in drawPointsAll"
        :cx="point.x"
        :cy="point.y"
        :r="real ? 8 : 5"
        fill="white"
        :stroke="isPointSelected(index) && real ? 'red' : 'black'"
        :stroke-width="isPointSelected(index) && real ? 3 : 1"
        :transform="transform()"
        :key="index"
        @mouseover="pointOver(index)"
        @mouseleave="pointLeave()"
        @mousedown="pointClick(index, real)"
    />
</template>

<style></style>
