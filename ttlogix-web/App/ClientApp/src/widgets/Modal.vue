<template>
    <transition :name="transition">
        <div :class="prefix + 'mask ' + containerClass" >
          <div :class="prefix + 'wrapper'">
            <div :class="prefix + 'container'" @keydown.esc.stop="$emit('exit')">
              <div :class="prefix + 'header'">
                <slot name="header">
                </slot>
              </div>
              <div :class="prefix + 'body'">
                <slot name="body">
                </slot>
              </div>
              <div :class="prefix + 'footer'">
                <slot name="footer">
                </slot>
              </div>
            </div>
          </div>
        </div>
    </transition>
</template>
<script>
    var num = 0;
    import { defineComponent } from 'vue';
export default defineComponent({
        name :'modal',
        props: {containerClass:null, enabled : {type : Boolean, default: true}},
        data() {
            return {
                prefix : this.enabled ? 'modal-' : 'page-content-',
                transition : this.enabled ? 'modal' : 'modal-disabled'
            }
        },
        mounted() {
            num++;
            if (num == 1) {
                document.body.classList.add('opened-modal')
            }
        },
        beforeUnmount() {
            num--
            if (num <= 0) {
                document.body.classList.remove('opened-modal')
            }
        }
    })
</script>
<style lang="scss">
.modal-header {

    > h2 {
        text-align: center;
    }
}
.modal-footer .pagination {
    padding: 0px !important;
}
.modal-footer {
  .button {
    margin-left: 10px;
  }
}
.modal-header .btn-bar, .modal-footer .btn-bar {
    padding: 0px !important;
}
</style>
<style scoped lang="scss">
.modal-mask {
  position: fixed;
  z-index: 200;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, .5);
  display: table;
  transition: opacity .3s ease;
}

.modal-wrapper {
  display: table-cell;
  vertical-align: middle;
}

.modal-container {
  min-width: 300px;
  max-width: 90vw;
  margin: 0px auto;
  padding: 20px 30px;
  background-color: #f3f3f3;
  border-radius: 2px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, .33);
  transition: all .3s ease;
  max-height: 100%;
  font-family: Helvetica, Arial, sans-serif;
}

.modal-header {
  margin-top: 0;
  font-size: 20px;
  color: black;
  padding-bottom: 10px;
}
.modal-header h2 {
    text-align: center;
 }
.modal-footer {
    text-align: right;
    padding-top: 10px;
    border-top: 1px solid silver;
    
}
.modal-grid .modal-header {
    border-bottom: 1px solid silver;
}

.modal-body {
  /*margin: 5px 0;*/
}

.modal-default-button {
  float: right;
}

.modal-enter {
  opacity: 0;
}

.modal-leave-active {
  opacity: 0;
}

.modal-enter .modal-container,
.modal-leave-active .modal-container {
  -webkit-transform: scale(1.1);
  transform: scale(1.1);
}
</style>