import Vue from 'vue'
import App from './App.vue'
////ElementUI
import ElementUI from 'element-ui'
Vue.use(ElementUI);
// import the component TreeView
import Treeselect from '@riophae/vue-treeselect'
import '@riophae/vue-treeselect/dist/vue-treeselect.css'

Vue.component("Treeselect", Treeselect);
//Datetime
import { Datetime } from 'vue-datetime'
// You need a specific loader for CSS files
import 'vue-datetime/dist/vue-datetime.css'
Vue.use(Datetime)
Vue.component("datetime", Datetime);
//// Dragger
import draggable from 'vuedraggable'
Vue.component('draggable', draggable);
Vue.config.productionTip = false;
new Vue({
    render: h => h(App),
}).$mount('#app');
