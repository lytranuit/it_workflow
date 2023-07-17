<template>
  <div class="text-center h-100">
    <embed
      :src="url"
      style="width: 100%; height: 100%"
      type="application/pdf"
    />
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
      return files[0].url;
    },
  },
  mounted() {},
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
