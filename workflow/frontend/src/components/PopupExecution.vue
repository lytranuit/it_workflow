<template>
  <li>
    <a href="#" @click="visible = true">
      <i class="far fa-paper-plane"></i>
      <span>Chạy quy trình</span>
    </a>
    <Dialog v-model:visible="visible" modal header="Chọn quy trình" style="" :breakpoints="{ '1199px': '75vw', '575px': '95vw' }">
      <div class="row">
        <div class="col-9">
          <div class="input-group">
            <span class="input-group-prepend">
              <button type="button" class="btn btn-gradient-secondary">
                <i class="fas fa-search"></i>
              </button>
            </span>
            <input type="text" id="example-input1-group2" name="example-input1-group2" class="form-control"
              placeholder="Tìm kiếm" v-model="search" v-on:keydown="change" />
          </div>
        </div>
        <div class="col-3">
          <select class="form-control form-control-sm" v-model="selected" v-on:change="change">
            <option value="-1">Tất cả các nhóm</option>
            <option v-for="option in groups" :key="option.id" :value="option.id">
              {{ option.name }}
            </option>
          </select>
        </div>
        <div class="col-12 mt-3">
          <div class="accordion" id="accordionselect">
            <div class="process-group" v-for="group in groups_filter" :key="group.id">
              <div class="h-acc" data-toggle="collapse" :data-target="'#group_' + group.id" aria-expanded="true">
                {{ group.name }}
              </div>
              <div :id="'group_' + group.id" class="collapse show" style="">
                <template v-for="process_version in group.list_process_version" :key="process_version.id">
                  <a class="process d-block" @click="execute(process_version.id)" v-if="!process_version.hidden">
                    {{ process_version.process.name }}
                  </a>
                </template>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Dialog>
  </li>
</template>

<script setup>
import Dialog from "primevue/dialog";
import { computed, onMounted, ref } from "vue";
import Api from "../api/Api";
import { useRouter } from "vue-router";
import moment from "moment";
const router = useRouter();
const visible = ref();
const selected = ref(-1);
const search = ref("");
const groups = ref([]);
const change = () => {
  var val = selected.value;
  var search_string = search.value ? search.value.toLowerCase() : "";
  groups.value = groups.value.map(function (item) {
    item.list_process_version = item.list_process_version.map(function (item1) {
      if (
        search_string == "" ||
        item1.process.name.toLowerCase().indexOf(search_string) != -1
      ) {
        item1.hidden = false;
      } else {
        item1.hidden = true;
      }
      return item1;
    });
    if (val == -1 || val == item.id) {
      item.hidden = false;
    } else {
      item.hidden = true;
    }
    if (!item.list_process_version.length) item.hidden = true;
    return item;
  });
};
const execute = (process_version_id) => {
  visible.value = false;
  router.push(
    "/execution/create/" + process_version_id + "?time=" + moment().valueOf()
  );
};
const groups_filter = computed(() => {
  return groups.value.filter((item) => {
    return item.list_process_version.length > 0 && !item.hidden;
  });
});
onMounted(() => {
  Api.ProcessGroupWithProcess().then((res) => {
    groups.value = res;
  });
});
</script>
