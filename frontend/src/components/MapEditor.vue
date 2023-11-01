<script setup>
import { onMounted } from 'vue';
import { ref, watch, nextTick } from 'vue';

const width = 700;
let id = 0;


const polygons = ref({});
const drawPoints = ref([]);
const polygonPoints = ref([]);
const randomPoints = ref([]);
const zones = ref([]);
const imageSrc = ref('/Duplex-0.svg');
const selectedZone = ref(null);
const toggle = ref(null);
const intersected = ref({});

const svg = ref(null);
const rect = ref(null);
const polygonsDom = ref(null);

onMounted(() => {
    nextTick(() => {
        rect.value = svg.value.getBoundingClientRect();
        console.log("Rect2", rect);
    });
});


watch(toggle, (newValue, oldValue) => {
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
});

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
        const x =
            ((e.clientX - rect.value.left) * width) /
            rect.value.width;
        const y =
            ((e.clientY - rect.value.top) * width) / //change to height
            rect.value.height;

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
    array.value.push(point);
};

const createPolygon = (points) => {
    if (points.length >= 3) {
        const pointString = points.map(point => `${point.x},${point.y}`).join(' ');
        let polygon = { "points": pointString, "id": id, stroke: "none" }
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
                console.log("Intersect", polygon.id);
                int[polygon.id].push(p);
            }
        }
    }
    intersected.value = int;
}


const viewBox = `0 0 ${width} ${width}`

</script>




<template>
    <v-btn-toggle v-model="toggle" color="info" variant="outlined">
        <v-btn value="startDraw">Start Drawing</v-btn>
        <v-btn value="endDraw" :disabled="toggle != 'startDraw' || drawPoints.length < 3">End Drawing</v-btn>
        <v-btn value="deleteZone" :disabled="selectedZone == null">Delete Zone</v-btn>
        <v-btn value="addMark">Add Marker</v-btn>
        <v-btn value="clear">Clear</v-btn>
    </v-btn-toggle>
    <svg @click="svgClick" ref="svg" id="my-svg" :width=width :height=width :viewBox="viewBox" class="border">
        <image :xlink:href="imageSrc" x="0" y="0" :width="width" :height="width" />
        <polygon v-for="(polygon, id) in polygons" ref="polygonsDom" :id="id" :points="polygon.points" fill="red"
            fill-opacity="0.5" @click="selectZone(id)" :stroke="polygon.stroke" />
        <circle v-for="point in drawPoints" :cx="point.x" :cy="point.y" r="3" fill="red" />
        <circle v-for="point in randomPoints" :cx="point.x" :cy="point.y" r="3" fill="red" />
    </svg>
    <p v-for="(points, id) in intersected" :key="id">
        Zone {{ id }}: {{ points.length }}
    </p>
</template>
  

  
<style scoped>
/* Add your component-specific styles here */
</style>
  