<template>
  <div :data-clazz="model.clazz">
    <div class="panelTitle">{{ $t("failEvent") }}</div>
    <div class="panelBody">
      <Accordion :activeIndex="0">
        <AccordionTab :header="$t('detail.general')">
          <DefaultDetail :model="model" :onChange="onChange" />
        </AccordionTab>
        <AccordionTab :header="$t('notification')">
          <div class="flex-m m-2">
            <div class="">Thông báo</div>
            <div class="ml-auto">
              <div class="custom-control custom-switch switch-primary">
                <input type="checkbox" class="custom-control-input" id="checknoti"
                  v-model="model.data_setting.has_notification" />
                <label class="custom-control-label" for="checknoti"></label>
              </div>
            </div>
          </div>
          <setting-mail :model="model"></setting-mail>
        </AccordionTab>
      </Accordion>
    </div>
  </div>
</template>
<script>
import DefaultDetail from "./DefaultDetail.vue";
import SettingMail from "./SettingMail.vue";
export default {
  components: {
    DefaultDetail,
    SettingMail,
  },
  props: {
    model: {
      type: Object,
      default: () => ({}),
    },
    onChange: {
      type: Function,
      default: () => { },
    },
    readOnly: {
      type: Boolean,
      default: false,
    },
  },
};
</script>
