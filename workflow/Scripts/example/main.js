import Vue from 'vue';
import App from './App.vue';
import PopupExecution from './PopupExecution.vue';
import Execution from './Execution.vue';
import User from './User.vue';
import store from "./store";
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
Vue.use(require('vue-moment'));
Vue.component("datetime", Datetime);
//// Dragger
import draggable from 'vuedraggable'
Vue.component('draggable', draggable);
Vue.config.productionTip = false;
if ($("#app").length) {
	new Vue({
		store,
		render: h => h(App)
	}).$mount('#app');
}
if ($("#popup-execution").length) {
	new Vue({
		store,
		render: h => h(PopupExecution)
	}).$mount('#popup-execution');
}
if ($("#execution").length) {
	new Vue({
		store,
		render: h => h(Execution)
	}).$mount('#execution');
}

if ($("#user").length) {
	new Vue({
		store,
		render: h => h(User)
	}).$mount('#user');
}
