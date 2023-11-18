import { createRouter, createWebHistory } from 'vue-router'
// import HomeView from "../views/HomeView.vue";
import Home from '@/views/HomeView.vue'
import Obra from '@/views/ObraView.vue'
import NotFound from '@/views/NotFoundView.vue'

import type { RouteRecordRaw } from 'vue-router'

const routes: Array<RouteRecordRaw> = [
    {
        path: '/',
        component: Home, // Set the layout component for all routes
        children: []
    },
    {
        path: '/obras/:id',
        component: Obra
    },
    { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router
