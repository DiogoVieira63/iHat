import { createRouter, createWebHistory } from 'vue-router'
// import HomeView from "../views/HomeView.vue";
import Home from '@/views/HomeView.vue'
import Obra from '@/views/ObraView.vue'
import NotFound from '@/views/NotFoundView.vue'


const routes = [
  {
    path: '/',
    component: Home, // Set the layout component for all routes
    children: [
      // { path: '/home', component: Home },
      // { path: 'about', component: About },
      // Add more routes as needed
    ]
  },
  {
    path: '/obras/:id',
    component: Obra
  },
  { path: '/:pathMatch(.*)*', name: 'NotFound', component: NotFound },
  // {
  //   path: "/",
  //   name: "home",
  //   component: HomeView,
  // },
  // {
  //   path: "/about",
  //   name: "about",
  //   // route level code-splitting
  //   // this generates a separate chunk (about.[hash].js) for this route
  //   // which is lazy-loaded when the route is visited.
  //   component: () =>
  //     import(/* webpackChunkName: "about" */ "../views/AboutView.vue"),
  // },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})


export default router
