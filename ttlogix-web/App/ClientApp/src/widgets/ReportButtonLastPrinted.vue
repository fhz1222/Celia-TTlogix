<template>
    <div style="display: inline-block" v-tooltip="tooltip">
        <button :disabled="disabled" class="btn btn-primary btn-sm me-2">
            <div style="display: inline-block;">
                <div>
                    <i class="las la-file-invoice"></i>
                    <slot></slot>
                </div>
                <div v-if="lastPrinted" style="color: #bfbfbf; font-size: 0.5;">
                    {{ lastPrinted }}
                </div>
            </div>
        </button>
    </div>
</template>
<script>

export default {
    props: ['lastPrinted', 'disabled', 'tooltip']
}
</script>