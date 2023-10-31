<script setup>
import { ref, watch } from 'vue';

const isDrawing = ref(false);
const addMarker = ref(false);
const polygonPoints = ref([]);
const zones = ref([]);
const selectedZone = ref(null);
const imageSrc = ref('/Duplex-0.svg');
const width = ref(700);
const toggle = ref(null);



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
    const svg = document.querySelector('#my-svg');
    console.log(svg, typeof (svg));
    if (isDrawing.value || addMarker.value) {
        const rect = svg.getBoundingClientRect();
        const x =
            ((e.clientX - rect.left) * svg.viewBox.baseVal.width) /
            rect.width;
        const y =
            ((e.clientY - rect.top) * svg.viewBox.baseVal.height) /
            rect.height;

        if (addMarker.value) {
            addMapMarker(x, y);
        }
        else{
            polygonPoints.value.push({ x, y });
            createPoint(x, y);
        }
    }
};
const createPoint = (x, y) => {
    const point = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    point.setAttribute('cx', x);
    point.setAttribute('cy', y);
    point.setAttribute('r', 3);
    point.setAttribute('fill', 'red');
    document.querySelector('#my-svg').appendChild(point);
};

const createPolygon = (points) => {
    if (points.length >= 3) {
        const polygon = document.createElementNS('http://www.w3.org/2000/svg', 'polygon');
        const pointString = points.map(point => `${point.x},${point.y}`).join(' ');
        polygon.setAttribute('points', pointString);
        polygon.setAttribute('fill', 'red');
        polygon.setAttribute('fill-opacity', '0.5');
        polygon.addEventListener('click', () => {
            selectZone(polygon);
        });
        selectZone(polygon);
        document.querySelector('#my-svg').appendChild(polygon);
        zones.value.push(polygon);
    }
};

const selectZone = (zone) => {
    if (selectedZone.value) {
        selectedZone.value.setAttribute('stroke', 'none');
    }
    if (selectedZone.value === zone) {
        selectedZone.value = null;
        return;
    }
    selectedZone.value = zone;
    selectedZone.value.setAttribute('stroke', 'yellow');
};

const deleteZone = () => {
    if (selectedZone.value) {
        document.querySelector('#my-svg').removeChild(selectedZone.value);
        zones.value = zones.value.filter(zone => zone !== selectedZone.value);
        selectedZone.value = null;
        toggle.value = null;
    }
};
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

const clearPoints = () => {
    const points = document.querySelectorAll('circle');
    points.forEach(point => document.querySelector('#my-svg').removeChild(point));
};

const viewBox = `0 0 ${width.value} ${width.value}`

</script>




<template>
    <h1>{{ toggle }} {{ isDrawing }}{{ addMarker }}</h1>
    <svg @click="svgClick" ref="svg" id="my-svg" :width=width :height=width :viewBox="viewBox">
        <image :xlink:href="imageSrc" x="0" y="0" :width="width" :height="width" />
    </svg>
    <v-btn-toggle v-model="toggle" divided color="info">
        <v-btn >Start Drawing</v-btn>
        <v-btn :disabled="toggle != 0">End Drawing</v-btn>
        <v-btn :disabled="selectedZone == null">Delete Zone</v-btn>
        <v-btn >Add Random Marker</v-btn>
    </v-btn-toggle>
</template>
  

  
<style scoped>
/* Add your component-specific styles here */
</style>
  