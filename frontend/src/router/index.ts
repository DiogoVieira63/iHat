import { createRouter, createWebHistory } from 'vue-router'
// import HomeView from "../views/HomeView.vue";
import Home from '@/views/HomeView.vue'
import Obra from '@/views/ObraView.vue'
import Capacete from '@/views/CapaceteView.vue'
import NotFound from '@/views/NotFoundView.vue'
import Simulator from '@/views/SimulatorView.vue'

import type { RouteRecordRaw } from 'vue-router'
import AboutView from '@/views/AboutView.vue'
import TeamView from '@/views/TeamView.vue'

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
    {
        path: '/capacetes/:id',
        component: Capacete
    },
    {
        path: '/obras/:id/simulador',
        component: Simulator
    },
    {
        path: '/about', 
        component: AboutView
    },
    {
        path: '/team', 
        component: TeamView
    },
    { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

export default router
