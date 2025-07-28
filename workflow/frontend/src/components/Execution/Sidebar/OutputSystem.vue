<template>
  <div class="text-center h-100">
    <div v-if="esign_id > 0 && hasPermission()" class="mb-2">
      <button class="mr-2 btn btn-primary" type="button" name="button" @click="setting()">
        <i class="fas fa-wrench"></i> Cấu hình trình ký
      </button>
      <button class="mr-2 btn btn-success" type="button" name="button" @click="suggets()">
        <i class="fas fa-sun"></i> Gợi ý vị trí ký
      </button>
    </div>
    <embed :src="url" style="width: 100%; height: 100%" type="application/pdf" />
  </div>
</template>
<script>
import { useAuth } from '../../../stores/auth';
import { useProcess } from '../../../stores/process';

var store = useProcess();
var store_auth = useAuth();
export default {
  components: {},
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
  },
  data() {
    return {};
  },
  computed: {
    selectedModel() {
      return store.selectedModel;
    },
    current_user() {
      return store_auth.user;
    },
    url() {
      var esign = this.model.data_setting.esign || {};
      var files = esign.files || [];
      return files[0].url;
    },
    esign_id() {
      return this.model.esign_id || 0;
    },


  },
  mounted() { },
  methods: {
    setting() {
      window.open(`https://esign.astahealthcare.com/admin/document/edit/${this.esign_id}`, "_blank");
    },
    suggets() {
      window.open(`https://esign.astahealthcare.com/admin/document/suggestsign/${this.esign_id}`, "_blank");
    },
    hasPermission() {
      // console.log(this.model);
      var current_user = this.current_user;
      var user_id = current_user.id;
      var created_by = this.model.created_by || 0;
      if (created_by == user_id) {
        return true;
      }
      return false;
    },

  },
};
</script>
<style lang="scss" scoped></style>
