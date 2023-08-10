<template>
  <div id="popup-require-sign">
    <div
      id="myModal-require-sign"
      class="modal"
      tabindex="-1"
      role="dialog"
      :data-backdrop="checkModal()"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Yêu cầu ký nháy</h5>
            <button
              type="button"
              class="close"
              data-dismiss="modal"
              aria-label="Close"
              v-if="!required"
            >
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          <div class="modal-body">
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
          </div>
          <div class="modal-footer justify-content-center">
            <button type="button" class="btn btn-success" @click="save()">
              Lưu lại
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import UserTreeSelect from "../TreeSelect/UserTreeSelect.vue";

export default {
  components: { UserTreeSelect },
  props: {
    activity: {
      type: Object,
      default: () => ({}),
    },
    required: {
      type: Boolean,
      default: () => false,
    },
  },
  data() {
    return {
      listusersign: [],
      //activeName: [0, 1, 2],
    };
  },
  computed: {},
  mounted() {
    var that = this;
    $("#myModal-require-sign").modal("show");
    $("#myModal-require-sign").on("hidden.bs.modal", function (e) {
      that.$emit("close");
    });
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
    checkModal() {
      var text = "static";
      if (!this.required) text = false;
      return text;
    },
  },
};
</script>

<style lang="scss"></style>
