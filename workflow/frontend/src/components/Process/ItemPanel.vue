<template>
  <div class="itemPanel" :style="{ height: height + 'px' }">
    <Accordion :activeIndex="0" multiple>
      <AccordionTab :header="$t('setting')" contentClass="text-center">
        <div class="form-group text-left">
          <div class="col-lg-12 mt-2">
            <b class="col-form-label">{{ $t("process.name") }}: <span class="text-danger">*</span></b>
            <div class="pt-1">
              <input class="form-control form-control-sm" type="text" name="name" required v-model="model.name" />
            </div>
          </div>
          <div class="col-lg-12 mt-2">
            <b class="col-form-label">{{ $t("process.group") }}:<span class="text-danger">*</span></b>
            <div class="pt-1">
              <TreeSelect v-model="model.group_id" :options="groups" :append-to-body="false" />
            </div>
          </div>
          <div class="col-lg-12 mt-2">
            <b class="col-form-label">{{ $t("process.code") }}: <span class="text-danger">*</span></b>
            <div class="pt-1">
              <input id="code" class="form-control form-control-sm" type="text" name="code" required
                v-model="model.code" />
            </div>
          </div>
          <div class="col-lg-12 mt-2">
            <b class="col-form-label">{{ $t("process.description") }}:</b>
            <div class="pt-1">
              <textarea id="description" class="form-control form-control-sm" v-model="model.description"></textarea>
            </div>
          </div>
        </div>
      </AccordionTab>
      <AccordionTab :header="$t('start')" contentClass="text-center">
        <img data-item="{clazz:'start',size:'30*30',label:''}" :src="'/images/flow/start.svg'"
          style="width: 42px; height: 42px" />
        <div>{{ $t("startEvent") }}</div>
        <!--<img data-item="{clazz:'timerStart',size:'30*30',label:''}"
             :src="'/images/flow/timer-start.svg'" style="width:42px;height:42px" />
        <div>{{$t('timerEvent')}}</div>-->
      </AccordionTab>
      <AccordionTab :header="$t('task')" contentClass="text-center">
        <img data-item="{clazz:'formTask',size:'80*44',label:''}" :src="'/images/flow/script-task.svg'"
          style="width: 80px; height: 44px" />
        <div>{{ $t("formTask") }}</div>
        <img data-item="{clazz:'approveTask',size:'80*44',label:''}" :src="'/images/flow/approve.svg'"
          style="width: 44px; height: 44px" />
        <div>{{ $t("approveTask") }}</div>
        <img data-item="{clazz:'suggestTask',size:'60*44',label:''}" :src="'/images/flow/suggest-task.svg'" style="
            width: 60px;
            height: 44px;
            padding: 5px 10px;
            border: 2px solid #c5c5c5;
            border-radius: 8px;
          " />
        <div>{{ $t("suggest") }}</div>
      </AccordionTab>
      <AccordionTab :header="$t('system')" contentClass="text-center">
        <img data-item="{clazz:'mailSystem',size:'80*44',label:''}" :src="'/images/flow/mail-task.svg'"
          style="width: 80px; height: 44px" />
        <div>{{ $t("mail") }}</div>
        <img data-item="{clazz:'printSystem',size:'50*60',label:''}" :src="'/images/flow/printSystem.svg'" style="
            width: 50px;
            height: 60px;
            padding: 5px 10px;
            border: 2px solid #c5c5c5;
            border-radius: 8px;
          " />
        <div>{{ $t("print") }}</div>

        <img data-item="{clazz:'outputSystem',size:'70*60',label:''}" :src="'/images/flow/output.png'" style="
            width: 70px;
            height: 60px;
            padding: 5px 10px;
            border: 1px solid #c5c5c5;
            border-radius: 8px;
          " />
        <div>{{ $t("output") }}</div>
      </AccordionTab>
      <AccordionTab :header="$t('gateway')" contentClass="text-center">
        <img data-item="{clazz:'exclusiveGateway',size:'40*40',label:''}" :src="'/images/flow/exclusive-gateway.svg'"
          style="width:48px;height:48px" />
        <div>{{ $t('exclusiveGateway') }}</div>
        <img data-item="{clazz:'parallelGateway',size:'40*40',label:''}" :src="'/images/flow/parallel-gateway.svg'"
          style="width: 48px; height: 48px" />
        <div>{{ $t("parallelGateway") }}</div>
        <img data-item="{clazz:'inclusiveGateway',size:'40*40',label:''}" :src="'/images/flow/inclusive-gateway.svg'"
          style="width: 48px; height: 48px" />
        <div>{{ $t("inclusiveGateway") }}</div>
      </AccordionTab>
      <AccordionTab :header="$t('end')" contentClass="text-center">
        <img data-item="{clazz:'fail',size:'30*30',label:''}" :src="'/images/flow/end.svg'"
          style="width: 42px; height: 42px" />
        <div>{{ $t("failEvent") }}</div>
        <img data-item="{clazz:'success',size:'30*30',label:''}" :src="'/images/flow/success.png'"
          style="width: 42px; height: 42px" />
        <div>{{ $t("successEvent") }}</div>
      </AccordionTab>
    </Accordion>
  </div>
</template>
<script setup>
import { useProcess } from "../../stores/process";
import { storeToRefs } from "pinia";
import { inject } from "vue";
const store = useProcess();
const { groups } = storeToRefs(store);
// console.log(groups);
const props = defineProps({
  height: {
    type: Number,
    default: 800,
  },
  model: {
    type: Object,
    default: () => ({}),
  },
});
</script>

<style lang="scss">
.itemPanel {
  float: left;
  width: 15%;
  background: #f0f2f5;
  overflow-y: auto;
  border-left: 1px solid #e9e9e9;
  border-bottom: 1px solid #e9e9e9;

  img {
    width: 92px;
    height: 96px;
    padding: 4px;
    border: 1px solid rgba(0, 0, 0, 0);
    border-radius: 2px;

    &:hover {
      border: 1px solid #ccc;
      cursor: move;
    }
  }
}
</style>
