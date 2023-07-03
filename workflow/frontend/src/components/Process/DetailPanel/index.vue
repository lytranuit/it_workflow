<template>
  <div class="detailPanel" :style="{ height: height + 'px' }">
    <FormTaskDetail
      v-if="model.clazz === 'formTask'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <GatewayDetail
      v-else-if="
        model.clazz === 'gateway' ||
        model.clazz === 'exclusiveGateway' ||
        model.clazz === 'parallelGateway' ||
        model.clazz === 'inclusiveGateway'
      "
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <FlowDetail
      v-else-if="model.clazz === 'flow'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <StartEventDetail
      v-else-if="model.clazz === 'start'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <FailEventDetail
      v-else-if="model.clazz === 'fail'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <SuccessEventDetail
      v-else-if="model.clazz === 'success'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <ApproveTaskDetail
      v-else-if="model.clazz === 'approveTask'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <SuggestTaskDetail
      v-else-if="model.clazz === 'suggestTask'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <MailDetail
      v-else-if="model.clazz === 'mailSystem'"
      :model="model"
      :onChange="onChange"
      :readOnly="readOnly"
    />
    <PrintDetail
      v-else-if="model.clazz === 'printSystem'"
      :model="model"
      :onChange="onChange"
    />
  </div>
</template>
<script>
import StartEventDetail from "./StartEventDetail.vue";
import FailEventDetail from "./FailEventDetail.vue";
import SuccessEventDetail from "./SuccessEventDetail.vue";

import ApproveTaskDetail from "./ApproveTaskDetail.vue";
import SuggestTaskDetail from "./SuggestTaskDetail.vue";
import FormTaskDetail from "./FormTaskDetail.vue";
import TimerEventDetail from "./TimerEventDetail.vue";

import GatewayDetail from "./GatewayDetail.vue";

import FlowDetail from "./FlowDetail.vue";

import MailDetail from "./MailDetail.vue";
import PrintDetail from "./PrintDetail.vue";

export default {
  inject: ["i18n"],
  components: {
    FormTaskDetail,
    TimerEventDetail,
    GatewayDetail,
    FlowDetail,
    StartEventDetail,
    FailEventDetail,
    SuccessEventDetail,
    ApproveTaskDetail,
    SuggestTaskDetail,
    MailDetail,
    PrintDetail,
  },
  props: {
    height: {
      type: Number,
      default: 800,
    },
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => {},
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
<style lang="scss">
.detailPanel {
  height: 100%;
  background: #f0f2f5;
  flex: 0 0 auto;
  float: left;
  width: 20%;
  border-right: 1px solid #e9e9e9;
  border-bottom: 1px solid #e9e9e9;
  border-left: 1px solid #e9e9e9;
  overflow: auto;

  .panelTitle {
    text-align: left;
    color: #000;
    background: #ebeef2;
    border-bottom: 1px solid #dce3e8;
    font-size: 16px;
    padding: 10px;
  }

  .panelBody {
    .panelRow {
      text-align: left;
      display: inline-block;
      font-size: 12px;
      width: 100%;
      padding: 5px 12px;
    }
  }
}
</style>
