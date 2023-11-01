<script setup>
import { onMounted } from 'vue';
import { ref, watch, nextTick } from 'vue';

const isDrawing = ref(false);
const addMarker = ref(false);
const polygonPoints = ref([]);
const zones = ref([]);
const selectedZone = ref(null);
const imageSrc = ref('/Duplex-0.svg');
const width = ref(700);
const toggle = ref(null);
const polygons = ref({});
const drawPoints = ref([]);
const svg = ref(null);
const rect = ref(null);
let id = 0;

onMounted(() => {
    nextTick(() => {
        rect.value = svg.value.getBoundingClientRect();
        console.log("Rect2", rect);
    });
});


watch(toggle, (newValue, oldValue) => {
    if (newValue === 0) {
        startDrawing();
    } else if (newValue === 1) {
        endDrawing();
    } else if (newValue === 2) {
        deleteZone();
    } else if (newValue === 3) {
        addMarker.value = true;
    }
});

const startDrawing = () => {
    isDrawing.value = true;
    polygonPoints.value = [];
};

const endDrawing = () => {
    isDrawing.value = false;
    createPolygon(polygonPoints.value);
    clearPoints();
    polygonPoints.value = [];
    toggle.value = null;
};


const svgClick = (e) => {
    if (isDrawing.value ) {
        const x =
            ((e.clientX - rect.value.left) * width.value) /
            rect.value.width;
        const y =
            ((e.clientY - rect.value.top) * width.value) / //change to height
            rect.value.height;

        polygonPoints.value.push({ x, y });
        createPoint(x, y);
    }
};
const createPoint = (x, y) => {
    const point = {}
    point['x'] = x
    point['y'] = y
    drawPoints.value.push(point);
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
    unselectZones();
    selectedZone.value = id;
    polygons.value[id].stroke = "yellow";
}



const deleteZone = () => {
    if (selectedZone.value) {
        delete polygons.value[selectedZone.value];
        selectedZone.value = null;
        toggle.value = null;
    }
};

/*
const addMapMarker = (x, y) => {
    // Create the SVG namespace
    const svgNS = "http://www.w3.org/2000/svg";

    // Create the marker SVG element
    const marker = document.createElementNS(svgNS, 'svg');

    // Set the marker attributes
    marker.setAttributeNS(null, 'width', '50px');
    marker.setAttributeNS(null, 'height', '50px');
    marker.setAttributeNS(null, 'viewBox', '0 0 24 24');
    marker.setAttributeNS(null, 'fill', 'none');

    // Create the first path of the marker
    const path1 = document.createElementNS(svgNS, 'path');
    path1.setAttributeNS(null, 'd', 'M12 13C13.6569 13 15 11.6569 15 10C15 8.34315 13.6569 7 12 7C10.3431 7 9 8.34315 9 10C9 11.6569 10.3431 13 12 13Z');
    path1.setAttributeNS(null, 'stroke', '#000000');
    path1.setAttributeNS(null, 'stroke-width', '2');
    path1.setAttributeNS(null, 'stroke-linecap', 'round');
    path1.setAttributeNS(null, 'stroke-linejoin', 'round');

    // Create the second path of the marker
    const path2 = document.createElementNS(svgNS, 'path');
    path2.setAttributeNS(null, 'd', 'M12 22C16 18 20 14.4183 20 10C20 5.58172 16.4183 2 12 2C7.58172 2 4 5.58172 4 10C4 14.4183 8 18 12 22Z');
    path2.setAttributeNS(null, 'stroke', '#000000');
    path2.setAttributeNS(null, 'stroke-width', '2');
    path2.setAttributeNS(null, 'stroke-linecap', 'round');
    path2.setAttributeNS(null, 'stroke-linejoin', 'round');

    // Append the paths to the marker
    marker.appendChild(path1);
    marker.appendChild(path2);

    // Set the position of the marker
    marker.setAttributeNS(null, 'x', x - 27);
    marker.setAttributeNS(null, 'y', y - 45);

    // Append the marker to the main SVG
    document.querySelector('#my-svg').appendChild(marker);
};
*/
const clearPoints = () => {
    drawPoints.value = [];
};

const viewBox = `0 0 ${width.value} ${width.value}`

</script>




<template>
    <v-row>
        <v-btn-toggle v-model="toggle" divided color="info">
            <v-btn>Start Drawing</v-btn>
            <v-btn :disabled="toggle != 0">End Drawing</v-btn>
            <v-btn :disabled="selectedZone == null">Delete Zone</v-btn>
            <!--v-btn >Add Random Marker</v-btn-->
        </v-btn-toggle>
    </v-row>
    <svg @click="svgClick" ref="svg" id="my-svg" :width=width :height=width :viewBox="viewBox" class="border">
        <image :xlink:href="imageSrc" x="0" y="0" :width="width" :height="width" />
        <g v-for="(polygon, id) in polygons">
            <polygon :points="polygon.points" fill="red" fill-opacity="0.5" @click="selectZone(id)"
                :stroke="polygon.stroke" />
        </g>
        <g v-for="point in drawPoints">
            <circle :cx="point.x" :cy="point.y" r="3" fill="red" />
        </g>
    </svg>
</template>
  

  
<style scoped>
/* Add your component-specific styles here */
</style>
  