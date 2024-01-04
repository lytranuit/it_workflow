<template>
  <div id="popup-assign">
    <Dialog v-model:visible="visible" header="Phân công" modal :closable="!required" class="p-fluid"
      :style="{ width: '50vw' }" :breakpoints="{ '1199px': '75vw', '575px': '95vw' }" @update:visible="close">
      <div class="row">
        <form id="form-assign" class="col-12">
          <Accordion :activeIndex="activeName" multiple>
            <AccordionTab :header="element.block.label" :key="index" v-for="(element, index) in data_custom_block">
              <div class="row bg-white">
                <!-- <div class="col-lg-3 mb-2">
                  <select class="form-control" :name="'sel_' + index" v-model="element.data_setting.type_performer"
                    required>
                    <option value="3">Bộ phận</option>
                    <option value="4">Người dùng</option>
                  </select>
                </div> -->
                <div class="col-lg-12 mb-2">
                  <div v-if="element.data_setting.type_performer == 4">
                    <UserDepartmentTreeSelect multiple required v-model="element.data_setting.listuser"
                      :name="'user_' + index">
                    </UserDepartmentTreeSelect>
                  </div>
                  <div v-if="element.data_setting.type_performer == 3">
                    <DepartmentTreeSelect multiple required v-model="element.data_setting.listdepartment"
                      :name="'dep_' + index"></DepartmentTreeSelect>
                  </div>
                </div>
              </div>
            </AccordionTab>
          </Accordion>
        </form>
      </div>
      <template #footer>
        <div class="text-center mt-3">
          <Button label="Hủy" icon="pi pi-times" class="p-button-sm p-button-danger mr-2" @click="close"
            v-if="!required"></Button>
          <Button label="Lưu lại" icon="pi pi-save" class="p-button-sm" @click="save"></Button>
        </div>
      </template>
    </Dialog>
  </div>
</template>
<script>
import Dialog from "primevue/dialog";
// import { useProcess } from "../../stores/process";
import DepartmentTreeSelect from "../TreeSelect/DepartmentTreeSelect.vue";
import UserDepartmentTreeSelect from "../TreeSelect/UserDepartmentTreeSelect.vue";
import Button from "primevue/button";
// var store = useProcess();
export default {
  components: { UserDepartmentTreeSelect, DepartmentTreeSelect, Dialog, Button },
  props: {
    data_custom_block: {
      type: Array,
      default: () => [],
    },
    required: {
      type: Boolean,
      default: () => true,
    },
  },
  data() {
    return {
      //activeName: [0, 1, 2],
    };
  },
  computed: {
    visible() {
      return this.data_custom_block.length > 0;
    },
    activeName() {
      return this.data_custom_block.map(function (item, key) {
        return key;
      });
    },
  },
  mounted() {
    for(var item of this.data_custom_block){
      item.data_setting.type_performer = 4;
    }
  },
  methods: {
    save() {
      var vaild = $("#form-assign").valid();
      if (!vaild) {
        return;
      }
      ////VAILD TIEU DE
      if (!$(".tieu_de").val()) {
        //alert("Bạn chưa nhập tiêu đề!");
        $(".tieu_de").focus();
        return;
      }
      this.$emit("save_data");
    },
    close() {
      this.$emit("close");
    },
  },
};
</script>

<style lang="scss"></style>
