<template>
  <div>
    <div class="item-control flex-m" v-for="element in fields" :key="element.id">
      <div class="mb-3 flex-m w-100">
        <div class="form-input-data-preview w-100">
          <div class="mb-2 form-input-control flex-wrap-m">
            <div class="custom-title-inline">
              <div class="container-label">
                <div class="container-left">
                  <div id="idControlName" class="font-weight-bold font-14 pr-2 d-inline-block">
                    <div style="">
                      {{ element.name }}
                      <span class="text-danger" style="float: inherit" v-if="element.is_require">
                        *
                      </span>
                    </div>
                  </div>
                  <div class="btn-selectTion-guide icon icon-infor-blue cursor-pointer icon-control"
                    style="color: #0c9cdd; float: right; margin-right: 5px" v-show="element.guide && element.guide != ''"
                    :v-tooltip="element.guide" placeholder="Bottom">
                    <i class="fas fa-info-circle" style="cursor: pointer"></i>
                  </div>
                </div>
              </div>
            </div>
            <div class="custom-field-inline">
              <div class="form-input-control-left w-100" v-if="element.type != 'table' && !readonly">
                <div v-if="element.type == 'number'">
                  <input @change="signalChange(element.id)" class="form-control form-control-sm number" type="number"
                    v-model="element.values.value" :required="element.is_require" :name="element.id" />
                </div>
                <div v-if="element.type == 'formular'">
                  <InputNumber class="w-100" v-model="element.values.value" :name="element.id" :pt="{
                    input: { required: element.is_require },
                  }" readonly="" v-bind="options_formular2(element.data_setting)" />
                </div>
                <div v-if="element.type == 'currency'">
                  <InputNumber class="w-100" :name="element.id" :required="element.is_require" :pt="{
                    input: { required: element.is_require },
                  }" v-model="element.values.value" :suffix="' ' + element.data_setting.currency || 'VND'"
                    locale="de-DE" :minFractionDigits="0" inputClass="p-inputtext-sm"
                    @update:model-value="signalChange(element.id)" />
                </div>

                <div v-if="element.type == 'text'">
                  <input class="form-control form-control-sm text" type="text" v-model="element.values.value"
                    :required="element.is_require" :name="element.id" />
                    
                </div>
                <div v-if="element.type == 'yesno'">
                  <div class="custom-control custom-switch switch-success">
                    <input type="checkbox" class="custom-control-input" :id="'customSwitch_' + element.id"
                      v-model="element.values.value" :name="element.id" value="1" />
                    <label class="custom-control-label" :for="'customSwitch_' + element.id"></label>
                  </div>
                </div>
                <div v-if="element.type == 'email'">
                  <input class="form-control form-control-sm email" type="email" v-model="element.values.value"
                    :required="element.is_require" :name="element.id" />
                </div>
                <div v-if="element.type == 'file'">
                  <input class="form-control form-control-sm file" type="file" :required="element.is_require"
                    :name="element.id" :accept="accept_file(element)" />
                </div>
                <div v-if="element.type == 'file_multiple'">
                  <input class="form-control form-control-sm file" type="file" :required="element.is_require"
                    :name="element.id" multiple :accept="accept_file(element)" />
                </div>
                <div v-if="element.type == 'date'">
                  <Calendar class="w-100" v-model="element.values.value" dateFormat="yy-mm-dd" :pt="{
                    input: { required: element.is_require },
                  }" :name="element.id" />
                </div>
                <div v-if="element.type == 'date_month'">
                  <Calendar class="w-100" v-model="element.values.value" view="month" dateFormat="yy-mm" :pt="{
                    input: { required: element.is_require },
                  }" :name="element.id" />
                </div>

                <div v-if="element.type == 'date_time'">
                  <Calendar class="w-100" :id="element.id" v-model="element.values.value" showTime hourFormat="24" :pt="{
                    input: { required: element.is_require },
                  }" :name="element.id" dateFormat="yy-mm-dd" :hideOnDateTimeSelect="true" :stepMinute="30"
                    :stepHour="1"></Calendar>
                </div>

                <div v-if="element.type == 'textarea'">
                  <textarea class="form-control form-control-sm textarea" v-model="element.values.value"
                    :required="element.is_require" :name="element.id"></textarea>
                </div>

                <div v-if="element.type == 'select'">
                  <TreeSelect :options="get_options(element.data_setting.options)" v-model="element.values.value"
                    :required="element.is_require" name="tran"></TreeSelect>
                </div>

                <div v-if="element.type == 'select_multiple'">
                  <TreeSelect :options="get_options(element.data_setting.options)" v-model="element.values.value_array"
                    multiple :required="element.is_require" name="tran2"></TreeSelect>
                </div>

                <div v-if="element.type == 'employee'">
                  <UserTreeSelect v-model="element.values.value" :required="element.is_require" :name="element.id">
                  </UserTreeSelect>
                </div>

                <div v-if="element.type == 'employee_multiple'">
                  <UserTreeSelect v-model="element.values.value_array" :required="element.is_require" :name="element.id"
                    multiple></UserTreeSelect>
                </div>
                <div v-if="element.type == 'radio'">
                  <div class="radio radio-success radio-circle" v-for="option in element.data_setting.options"
                    :key="option.id">
                    <input :id="'radio-' + option.id" type="radio" :name="'radio-' + element.id" :value="option.id"
                      v-model="element.values.value" />
                    <label :for="'radio-' + option.id">
                      {{ option.name }}
                    </label>
                  </div>
                </div>

                <div v-if="element.type == 'checkbox'">
                  <div class="checkbox checkbox-success" v-for="option in element.data_setting.options" :key="option.id">
                    <input :id="'checkbox-' + option.id" type="checkbox" :value="option.id"
                      v-model="element.values.value_array" />
                    <label :for="'checkbox-' + option.id">
                      {{ option.name }}
                    </label>
                  </div>
                </div>
                <div v-if="element.type == 'department'">
                  <DepartmentTreeSelect v-model="element.values.value" :required="element.is_require" :name="element.id">
                  </DepartmentTreeSelect>
                </div>

                <div v-if="element.type == 'department_multiple'">
                  <DepartmentTreeSelect v-model="element.values.value_array" :required="element.is_require"
                    :name="element.id" multiple></DepartmentTreeSelect>
                </div>
                <div v-if="element.type == 'task'">
                  <div class="checkbox checkbox-success checkbox-circle" v-for="element in element.data_setting.options"
                    :key="element.id">
                    <input :id="'task-' + element.id" type="checkbox" />
                    <label :for="'task-' + element.id">
                      {{ element.name }}
                    </label>
                  </div>
                </div>
              </div>
              <div class="w-100" v-else v-html="display(element)"></div>
            </div>
          </div>
          <div v-if="element.type == 'table'">
            <table class="table table-bordered mb-0 bg-white" style="outline: 1px solid #dee2e6 !important">
              <thead class="">
                <tr>
                  <th class="border-top-0 text-center" v-for="column in element.data_setting.columns" :key="column.id">
                    {{ column.name }}
                    <span class="text-danger" v-if="column.is_require">*</span>
                  </th>
                  <th v-if="!readonly && element.values.list_data.length > 1"></th>
                </tr>
              </thead>
              <tbody v-if="!readonly">
                <tr v-for="(row, index1) in element.values.list_data" :key="index1">
                  <td v-for="column in element.data_setting.columns" :key="column.id" class="text-center">
                    <div v-if="column.type == 'stt'">
                      {{ row[column.id] }}
                    </div>
                    <div v-if="column.type == 'formular'">
                      <InputNumber class="w-100" v-model="row[column.id]" :name="column.id + '_' + index1" :pt="{
                        input: { required: column.is_require },
                      }" readonly="" v-bind="options_formular2(column)" />
                    </div>
                    <div v-if="column.type == 'number'">
                      <input class="form-control form-control-sm number" type="number" v-model="row[column.id]"
                        :name="column.id + '_' + index1" :required="column.is_require" @change="
                          signalChange(
                            column.id,
                            index1,
                            element.data_setting.columns,
                            element.values.list_data
                          )
                          " />
                    </div>
                    <div v-if="column.type == 'currency'">
                      <InputNumber class="w-100" v-model="row[column.id]" :name="column.id + '_' + index1" :pt="{
                        input: { required: column.is_require },
                      }" :suffix="' ' + column.currency || 'VND'" locale="de-DE" :minFractionDigits="0"
                        inputClass="p-inputtext-sm" @update:model-value="
                          signalChange(
                            column.id,
                            index1,
                            element.data_setting.columns,
                            element.values.list_data
                          )
                          " />
                    </div>
                    <div v-if="column.type == 'text'">
                      <input class="form-control form-control-sm text" type="text" v-model="row[column.id]"
                        :name="column.id + '_' + index1" :required="column.is_require" />
                    </div>
                    <div v-if="column.type == 'yesno'">
                      <div class="custom-control custom-switch switch-success">
                        <input type="checkbox" class="custom-control-input"
                          :id="'customSwitch_' + column.id + '_' + index1" v-model="row[column.id]"
                          :name="column.id + '_' + index1" value="1" />
                        <label class="custom-control-label" :for="'customSwitch_' + column.id + '_' + index1"></label>
                      </div>
                    </div>
                    <div v-if="column.type == 'email'">
                      <input class="form-control form-control-sm email" type="email" v-model="row[column.id]"
                        :name="column.id + '_' + index1" :required="column.is_require" />
                    </div>
                    <div v-if="column.type == 'date'">
                      <Calendar class="w-100" v-model="row[column.id]" dateFormat="yy-mm-dd" :pt="{
                        input: { required: column.is_require },
                      }" :name="column.id + '_' + index1" />
                    </div>
                    <div v-if="column.type == 'date_month'">
                      <Calendar class="w-100" v-model="row[column.id]" view="month" dateFormat="yy-mm" :pt="{
                        input: { required: column.is_require },
                      }" :name="column.id + '_' + index1" />
                    </div>

                    <div v-if="column.type == 'date_time'">
                      <Calendar class="w-100" id="calendar-24h" v-model="row[column.id]" showTime hourFormat="24" :pt="{
                        input: { required: column.is_require },
                      }" :name="column.id + '_' + index1" :manualInput="true"></Calendar>
                    </div>
                    <div v-if="column.type == 'textarea'">
                      <textarea class="form-control form-control-sm textarea" v-model="row[column.id]"
                        :name="column.id + '_' + index1" :required="column.is_require"></textarea>
                    </div>
                  </td>
                  <td v-if="element.values.list_data.length > 1">
                    <div class="ml-2 text-danger" style="cursor: pointer" @click="remove_row(element, index1)">
                      <i class="fas fa-trash-alt"></i>
                    </div>
                  </td>
                </tr>
                <tr>
                  <td :colspan="element.data_setting.columns.length">
                    <a href="#" style="color: #1e9dc3" @click="add_row_table(element)" ref="addRowTable"
                      :data-count="element.values.list_data.length"><i class="fas fa-plus"></i> Thêm dòng</a>
                  </td>
                </tr>
              </tbody>
              <tbody v-else>
                <tr v-for="(row, index1) in element.values.list_data" :key="index1">
                  <td v-for="column in element.data_setting.columns" :key="column.id" class="text-center">
                    <div v-if="column.type == 'currency'">
                      {{ format_currency(row[column.id], column.currency) }}
                    </div>
                    <div v-else-if="column.type == 'formular'">
                      {{
                        format_formular(
                          row[column.id],
                          options_formular(column)
                        )
                      }}
                    </div>
                    <div v-else-if="column.type == 'yesno'">
                      <i v-if="row[column.id] == 'true'" class="far fa-check-circle text-success"></i>
                      <i v-if="row[column.id] != 'true'" class="fas fa-ban text-danger"></i>
                    </div>
                    <div v-else>{{ row[column.id] }}</div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
// import mitt from "mitt";

import stringMath from "string-math";
// const emitter = mitt();
import InputNumber from "primevue/inputnumber";
import { rand } from "../../../utilities/rand";
import moment from "moment";
import UserTreeSelect from "../../TreeSelect/UserTreeSelect.vue";
import DepartmentTreeSelect from "../../TreeSelect/DepartmentTreeSelect.vue";
import { useAuth } from "../../../stores/auth";

var store_auth = useAuth();
export default {
  components: {
    InputNumber,
    UserTreeSelect,
    DepartmentTreeSelect,
  },
  props: {
    fields: {
      type: Array,
      default: () => [],
    },
    users: {
      type: Array,
      default: () => [],
    },
    nodes: {
      type: Array,
      default: () => [],
    },
    departments: {
      type: Array,
      default: () => [],
    },
    readonly: {
      type: Boolean,
      default: () => false,
    },
  },
  data() {
    return {
      change: 1,
    };
  },
  computed: {
    current_user() {
      return store_auth.user;
    },
  },
  mounted() {
    var index = 0;
    $(".vue-TreeSelect__input").each(function () {
      $(this).attr("name", "index_" + index++);
    });
    if (this.$refs.addRowTable) {
      this.$refs.addRowTable.map(function (item) {
        if ($(item).data("count") == 0) {
          item.click();
        }
      });
    }
    this.fill_field();
    // console.log(this.fields);
  },
  methods: {
    fill_field() {
      var firstvariable = "!#"; //first input;
      var secondvariable = "#"; //first in
      var fields = this.fields;
      if (this.readonly == false && fields) {
        for (var field of fields) {
          if (field.variable == "created_by_ngaynghi") {
            field.values.value = this.current_user.ngaynghi ? this.current_user.ngaynghi : 0;
          }
          var list_data = field.values.list_data || [];
          // console.log(list_data);
          if (field.type == "table" && field.data_setting.columns) {
            var columns = field.data_setting.columns;
            for (var column of columns) {
              if (column.type == "formular") {
                var text = column.formular.text;
                var list_id = text.match(
                  new RegExp(
                    "(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")",
                    "g"
                  )
                );
                for (var id of list_id) {
                  $("input[name='" + id + "']").trigger("change");
                }
              }
            }
          }
          if (field.type == "formular") {
            if (
              field.data_setting.formular.type == 2
            ) {

              var operator_column = field.data_setting.formular.operator_column;
              var operator_type = field.data_setting.formular.operator_type;
              var arr = list_data.map(function (item) {
                return item[operator_column];
              });
              switch (operator_type) {
                case "sum":
                  field.values.value = arr.reduce((a, b) => a + b, 0);
                  //console.log(arr);
                  break;
                case "avg":
                  field.values.value =
                    arr.length > 0
                      ? arr.reduce((a, b) => a + b, 0) / arr.length
                      : 0;
                  break;
                case "min":
                  field.values.value = Math.min(...arr);
                  break;
                case "max":
                  field.values.value = Math.max(...arr);
                  break;
              }
            }
            if (field.data_setting.formular.type == 1) {
              var text1 = field.data_setting.formular.text;
              var list_id1 = text1.match(
                new RegExp(
                  "(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")",
                  "g"
                )
              );

              // console.log(list_id1);
              // if (list_id1.indexOf(id) == -1) {
              //   continue;
              // }
              for (let _id of list_id1) {
                var findindex = this.fields.findIndex(function (item) {
                  return item.id == _id;
                });
                var findField = this.fields[findindex];
                // console.log(this.fields);
                // console.log(_id);
                let value = findField.values.value ? findField.values.value : 0;
                text1 = text1.replace(new RegExp("!#" + _id + "#", "g"), value);
              }
              var result1 = stringMath(text1, function () {
                return 0;
              });
              // console.log(result1);
              field.values.value = result1;
            }
          }


        }
      }
    },
    accept_file(element) {
      var data_setting = element.data_setting;
      var accept = data_setting.accept_file;
      var ret = null;
      if (accept == "pdf") {
        ret = "application/pdf"
      } else if (accept == "image") {
        ret = "image/*"
      }
      return ret;
    },
    options_formular(column) {
      var data_return = {
        style: column.formular.type_return,
        minimumFractionDigits: column.formular.decimal_number
      };
      if (column.formular.type_return == "currency") {
        data_return.currency = "VND";
      }
      return data_return;
    },

    options_formular2(column) {
      var suffix = "";
      if (column.formular.type_return == "percent") {
        suffix = " %";
      } else if (column.formular.type_return == "currency") {
        suffix = " VND";
      }
      // console.log(column.formular.decimal_number);
      return {
        suffix: suffix,
        locale: "de-DE",
        minFractionDigits: column.formular.decimal_number || 0,
        inputClass: "p-inputtext-sm",
      };
    },

    signalChange: function (id, index = null, columns = [], list_data = []) {
      //console.log(evt.target);
      //var name = $(evt.target).attr("name");
      //var list = name.split("_");
      //var id = list[0];
      //var index = list[1];
      var firstvariable = "!#"; //first input;
      var secondvariable = "#"; //first in
      if (index != null) {
        //// CHANGE IN TABLE
        var row = list_data[index];
        for (var column of columns) {
          if (column.type == "formular") {
            var column_id = column.id;
            var text = column.formular.text;
            var list_id = text.match(
              new RegExp(
                "(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")",
                "g"
              )
            );
            console.log(list_id)
            //console.log(text)
            if (list_id.indexOf(id) == -1) {
              continue;
            }
            for (var _id of list_id) {
              var value = row[_id] ? row[_id] : 0;
              text = text.replace(new RegExp("!#" + _id + "#", "g"), value);
            }
            var result = stringMath(text, function () {
              return 0;
            });
            // console.log(column);
            row[column_id] = result;
            this.signalChange(column_id, index, columns, list_data);
            //console.log(text);
            //console.log(row[column_id]);
          }
        }
      }
      //// CHANGE IN FIELD
      var field_formular = this.fields.filter(function (item) {
        return item.type == "formular";
      });
      // console.log(field_formular);
      if (field_formular) {
        for (var field of field_formular) {
          if (
            field.data_setting.formular.type == 2 &&
            id == field.data_setting.formular.operator_column
          ) {
            var operator_type = field.data_setting.formular.operator_type;
            var arr = list_data.map(function (item) {
              return item[id];
            });
            switch (operator_type) {
              case "sum":
                field.values.value = arr.reduce((a, b) => a + b, 0);
                //console.log(arr);
                break;
              case "avg":
                field.values.value =
                  arr.length > 0
                    ? arr.reduce((a, b) => a + b, 0) / arr.length
                    : 0;
                break;
              case "min":
                field.values.value = Math.min(...arr);
                break;
              case "max":
                field.values.value = Math.max(...arr);
                break;
            }
          }
          if (field.data_setting.formular.type == 1) {
            var text1 = field.data_setting.formular.text;
            var list_id1 = text1.match(
              new RegExp(
                "(?<=" + firstvariable + ")(.*?)(?=" + secondvariable + ")",
                "g"
              )
            );

            // console.log(list_id1);
            if (list_id1.indexOf(id) == -1) {
              continue;
            }
            for (let _id of list_id1) {
              var findindex = this.fields.findIndex(function (item) {
                return item.id == _id;
              });
              var findField = this.fields[findindex];
              let value = findField.values.value ? findField.values.value : 0;
              text1 = text1.replace(new RegExp("!#" + _id + "#", "g"), value);
            }
            var result1 = stringMath(text1, function () {
              return 0;
            });
            field.values.value = result1;
          }
        }
      }
      this.$forceUpdate();
    },

    get_options(options) {
      return options.map(function (item) {
        item.label = item.name;
        return item;
      });
    },
    add_row_table(field) {
      var columns = field.data_setting.columns;
      var data = {};
      var column_stt = null;
      for (var column of columns) {
        if (column.type == "stt") {
          column_stt = column.id;
        } else {
          data[column.id] = null;
        }
      }
      field.values.list_data.push(data);
      if (column_stt) {
        field.values.list_data = field.values.list_data.map(function (
          item,
          key
        ) {
          item[column_stt] = key + 1;
          return item;
        });
      }
      var name = field.name;
      field.name = rand();
      field.name = name;
    },
    remove_row(element, index) {
      element.values.list_data.splice(index, 1);

      var columns = element.data_setting.columns;
      var column_stt = null;
      for (var column of columns) {
        if (column.type == "stt") {
          column_stt = column.id;
        }
      }
      if (column_stt) {
        element.values.list_data = element.values.list_data.map(function (
          item,
          key
        ) {
          item[column_stt] = key + 1;
          return item;
        });
      }
      var name = element.name;
      element.name = rand();
      element.name = name;
    },
    display(field) {
      var text = field.values.value;
      var data_setting = field.data_setting;
      if (field.type == "select" || field.type == "radio") {
        var options = data_setting.options;
        var index = options.findIndex(function (item) {
          return item.id == field.values.value;
        });
        if (index != -1) {
          var option = options[index];
          text = option.name;
        }
      } else if (field.type == "file" || field.type == "file_multiple") {
        text = "";
        if (field.values.files) {
          for (var file of field.values.files) {
            text +=
              `
                                <div class="flex-m mb-1">
                                    <div class="file-icon" data-type="` +
              file.ext +
              `"></div>
                                    <a href="` +
              file.url +
              `" download="` +
              file.name +
              `" style="margin-left: 5px;">
							            ` +
              file.name +
              `
						            </a>
                                </div>`;
          }
        }
      } else if (field.type == "department") {
        var index = this.departments.findIndex(function (item) {
          return item.id == field.values.value;
        });
        if (index != -1) {
          var option = this.departments[index];
          text = option.label;
        }
      } else if (field.type == "employee") {
        var index = this.users.findIndex(function (item) {
          return item.id == field.values.value;
        });
        if (index != -1) {
          var option = this.users[index];
          text = option.label;
        }
      } else if (field.type == "select_multiple" || field.type == "checkbox") {
        var value_array = field.values.value_array || [];
        var options = data_setting.options;
        var list = options
          .filter(function (item) {
            return value_array.indexOf(item.id) != -1;
          })
          .map(function (item) {
            return item.name;
          });
        text = list.join(", ");
      } else if (field.type == "select_department") {
        var value_array = field.values.value_array || [];
        var list = this.departments
          .filter(function (item) {
            return value_array.indexOf(item.id) != -1;
          })
          .map(function (item) {
            return item.label;
          });
        text = list.join(", ");
      } else if (field.type == "select_employee") {
        var value_array = field.values.value_array || [];
        var list = this.users
          .filter(function (item) {
            return value_array.indexOf(item.id) != -1;
          })
          .map(function (item) {
            return item.label;
          });
        text = list.join(", ");
      } else if (field.type == "date") {
        text = field.values.value
          ? moment(field.values.value).format("YYYY-MM-DD")
          : "";
      } else if (field.type == "date_month") {
        text = field.values.value
          ? moment(field.values.value).format("YYYY-MM")
          : "";
      } else if (field.type == "date_time") {
        text = field.values.value
          ? moment(field.values.value).format("YYYY-MM-DD HH:mm")
          : "";
      } else if (field.type == "currency") {
        text = field.values.value
          ? new Intl.NumberFormat("de-DE", {
            style: "currency",
            currency: data_setting.currency,
          }).format(field.values.value)
          : "";
      } else if (field.type == "formular") {
        text = field.values.value
          ? this.format_formular(
            field.values.value,
            this.options_formular(data_setting)
          )
          : "";
      } else if (field.type == "yesno") {
        //console.log(field.values.value);
        text =
          field.values.value && field.values.value == "true"
            ? "<span class='text-success'><i class='far fa-check-circle'></i> Chọn</span>"
            : "<span class='text-danger'><i class='fas fa-ban'></i> Không chọn</span>";
      }
      return text;
    },
    format_currency(value, currency) {
      return new Intl.NumberFormat("de-DE", {
        style: "currency",
        currency: currency,
      }).format(value);
    },

    format_formular(value, option) {
      // console.log(value);
      return new Intl.NumberFormat("de-DE", option).format(value)
    },
  },
};
</script>
<style lang="scss" scoped></style>
