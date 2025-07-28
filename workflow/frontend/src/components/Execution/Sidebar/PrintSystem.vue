<template>
  <div class="text-center h-100">

    <embed :src="url" style="width: 100%; height: 100%" type="application/pdf" v-if="type_output == 'pdf'" />
    <div class="flex-m mb-1" v-else>
      <div>File: </div>
      <a :href="url" :download="file.name" style="margin-left: 5px;">
        {{ file.name }}
      </a>
    </div>
  </div>
</template>
<script>
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
    url() {
      var esign = this.model.data_setting.esign || {};
      var files = esign.files || [];
      if (files.length == 0) {
        return "";
      }
      return files[0].url;
    },
    file() {
      var esign = this.model.data_setting.esign || {};
      var files = esign.files || [];
      if (files.length == 0) {
        return {};
      }
      return files[0];
    },
    type_output() {
      return this.model.data_setting.type_output || "pdf";
    },
  },
  mounted() { },
  methods: {
    //signatures() {
    //    var esign = this.model.data_setting.esign || {};
    //    var block_id = this.model.block_id;
    //    var blocks_approve = this.nodes.filter(function (item) {
    //        return item.data_setting.blocks_esign_id == block_id;
    //    })
    //    var signatures = [];
    //    console.log(blocks_approve);
    //    console.log(this.nodes);
    //    for (var block of blocks_approve) {
    //        signatures.push({
    //            block_id: block.id,
    //            name: block.label,
    //        });
    //    }
    //    //return signatures;
    //}
  },
};
</script>
<style lang="scss" scoped></style>
