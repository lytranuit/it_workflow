<template>
  <div id="popup-require-sign">
    <Dialog
      v-model:visible="visible"
      header="Yêu cầu ký tên"
      modal
      class="p-fluid"
      style="width: 50vw"
      @update:visible="close"
    >
      <div class="row">
        <form id="form-require-sign" class="col-12">
          <div class="row bg-white flex-m">
            <div class="col-3">Người thực hiện:</div>
            <div class="col-9">
              <UserTreeSelect
                multiple
                required
                v-model="listusersign"
                name="user_sign"
              ></UserTreeSelect>
            </div>
          </div>
        </form>
      </div>
      <template #footer>
        <div class="text-center mt-3">
          <Button
            label="Hủy"
            icon="pi pi-times"
            class="p-button-sm p-button-danger"
            @click="close"
          ></Button>
          <Button
            label="Lưu lại"
            icon="pi pi-save"
            class="p-button-sm"
            @click="save"
          ></Button>
        </div>
      </template>
    </Dialog>
  </div>
</template>
<script>
import Button from "primevue/button";
import UserTreeSelect from "../TreeSelect/UserTreeSelect.vue";
import Dialog from "primevue/dialog";

export default {
  components: { UserTreeSelect, Button, Dialog },
  props: {
    activity: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {
      listusersign: [],
      //activeName: [0, 1, 2],
    };
  },
  computed: {
    visible() {
      return this.activity;
    },
  },
  mounted() {
    console.log(this.visible);
  },
  methods: {
    save() {
      var vaild = $("#form-require-sign").valid();
      if (!vaild) {
        return;
      }
      var list = [];
      for (var user of this.listusersign) {
        var obj = {
          user_sign: user,
          status: 1,
        };
        list.push(obj);
      }
      this.activity.data_setting.listusersign = list;
      this.activity.is_update = true;
      this.activity.event_type = "require_sign";
      this.$emit("save_data");
    },
    close() {
      this.$emit("close");
    },
  },
};
</script>

<style lang="scss"></style>
