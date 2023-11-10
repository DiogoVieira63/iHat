<script setup >
import { onMounted } from 'vue';
import { computed } from 'vue';
import { ref, watch, nextTick, onUpdated} from 'vue';
import { useDisplay } from 'vuetify'

let id = 0;

const props = defineProps({
    svgSrc: {
        type: String,
        required: true,
    },
    edit: {
        type: Boolean,
        required: true,
    },
    active: {
        type: Boolean,
        required: true,
    }
});

const coef = 0.6;
const baseWidth = 1052 * coef;
const baseHeight = 1488 * coef;
const ratio = baseWidth / baseHeight;
const coefSvg = ref(1)

const svgWidth = ref(baseWidth);
const svgHeight = ref(baseHeight)
const { width, height } = useDisplay()

const polygons = ref({});
const drawPoints = ref([]);
const polygonPoints = ref([]);
const randomPoints = ref([]);
const zones = ref([]);
const selectedZone = ref(null);
const toggle = ref(null);
const intersected = ref({});

const svg = ref(null);
const polygonsDom = ref(null);


const updateEditButton = ((newValue) => {
    if (newValue === "startDraw") {
        startDrawing();
    } else if (newValue === "endDraw") {
        endDrawing();
        toggle.value = null;
    } else if (newValue === "deleteZone") {
        deleteZone();
        toggle.value = null;
    } else if (newValue === "clear") {
        randomPoints.value = [];
        drawPoints.value = [];
        polygons.value = {};
        zones.value = [];
        selectedZone.value = null;
        intersected.value = {};
        toggle.value = null;
    }
})

onUpdated(() => {
    console.log("Updated " + props.svgSrc);
})

const startDrawing = () => {
    polygonPoints.value = [];
};

const endDrawing = () => {
    createPolygon(polygonPoints.value);
    clearPoints();
    polygonPoints.value = [];
};


const svgClick = (e) => {
    if (toggle.value === "startDraw" || toggle.value === "addMark") {
        const x = e.offsetX;
        const y = e.offsetY;
        if (toggle.value === "startDraw") {
            polygonPoints.value.push({ x, y });
            createPoint(drawPoints, x, y);
        }
        else if (toggle.value === "addMark") {
            createPoint(randomPoints, x, y);
            checkIfIntersect();
        }
    }
};
const createPoint = (array, x, y) => {
    const point = {}
    point['x'] = x
    point['y'] = y
    point['coef'] = coefSvg.value
    array.value.push(point);
};

const createPolygon = (points) => {
    if (points.length >= 3) {
        const pointString = points.map(point => `${point.x},${point.y}`).join(' ');
        let polygon = { "points": pointString, "id": id, stroke: "none", coef: coefSvg.value }
        polygons.value[id] = polygon;
        zones.value.push(polygon);
        selectZone(id);
        id += 1;
    }
};

const unselectZones = () => {
    for (const [key, value] of Object.entries(polygons.value)) {
        polygons.value[key].stroke = "none";
    }
}

const selectZone = (id) => {
    if (toggle.value === "startDraw" || toggle.value === "addMark") {
        return;
    }
    unselectZones();
    selectedZone.value = id;
    polygons.value[id].stroke = "yellow";
}



const deleteZone = () => {
    if (selectedZone.value) {
        delete polygons.value[selectedZone.value];
        delete intersected.value[selectedZone.value];
        selectedZone.value = null;
    }
};


const clearPoints = () => {
    drawPoints.value = [];
};

const checkIfIntersect = () => {
    // iterate to randomPoints
    let int = {};
    for (let j = 0; j < polygonsDom.value.length; j++) {
        let polygon = polygonsDom.value[j];
        int[polygon.id] = []
        for (let i = 0; i < randomPoints.value.length; i++) {
            let p = randomPoints.value[i];
            // iterate to polygons
            let point = svg.value.createSVGPoint();
            point.x = p.x;
            point.y = p.y;
            if (polygon.isPointInFill(point) || polygon.isPointInStroke(point)) {
                int[polygon.id].push(p);
            }
        }
    }
    intersected.value = int;
}

const onResize = () => {
    let coefX = width.value / 2 / baseWidth;
    svgWidth.value = coefX > 1 ? baseWidth : baseWidth * coefX;
    svgHeight.value = svgWidth.value / ratio;
    coefSvg.value = svgWidth.value / baseWidth;
}


const viewBox = computed(() => {
    return `0 0 ${svgWidth.value} ${svgHeight.value}`
})

function transform(value) {
    if (!value) {
        return `scale(${coefSvg.value})`
    }
    let res = coefSvg.value / value;
    return `scale(${res})`
}


</script>




<template>
    <v-container v-if="active">
        <v-row justify="center">
            <svg @click="svgClick" ref="svg" id="my-svg" :width=svgWidth :height=svgHeight :viewBox="viewBox"
                v-resize="onResize" class="border justify-center">
                <image :xlink:href="props.svgSrc" x="0" y="0" :width="svgWidth" :height="svgHeight" />
                <polygon v-for="(polygon, id) in polygons" ref="polygonsDom" :id="id" :points="polygon.points" fill="red"
                    fill-opacity="0.5" @click="selectZone(id)" :stroke="polygon.stroke"
                    :transform="transform(polygon.coef)" />
                <circle v-for="point in drawPoints" :cx="point.x" :cy="point.y" r="3" fill="red"
                    :transform="transform(point.coef)" />
                <circle v-for="point in randomPoints" :cx="point.x" :cy="point.y" r="3" fill="red"
                    :transform="transform(point.coef)" />
            </svg>
        </v-row>
        <v-row v-if="props.edit" justify="center">
            <v-btn-toggle v-model="toggle" color="info" variant="outlined" @update:model-value="updateEditButton">
                <v-tooltip text="Start Polygon">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-play" value="startDraw"></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="End Polygon">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-plus" value="endDraw"
                            :disabled="toggle != 'startDraw' || drawPoints.length < 3"></v-btn>
                    </template>
                </v-tooltip> <v-tooltip text="Delete Zone">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-delete" value="deleteZone" :disabled="selectedZone == null"></v-btn>
                    </template>
                </v-tooltip> <v-tooltip text="Add Marker">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-map-marker" value="addMark"></v-btn>
                    </template>
                </v-tooltip>
                <v-tooltip text="Clear">
                    <template v-slot:activator="{ props }">
                        <v-btn v-bind="props" icon="mdi-broom" value="clear"></v-btn>
                    </template>
                </v-tooltip>
            </v-btn-toggle>
        </v-row>
    </v-container>
</template>
  

  
<style scoped>
/* Add your component-specific styles here */
</style>
  