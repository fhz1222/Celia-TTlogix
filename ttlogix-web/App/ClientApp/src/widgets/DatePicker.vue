<script lang="ts" setup>
    import { ref, onMounted, watch } from 'vue';
    import moment from 'moment';

    class Format {
        screen: string;
        server: string;
        todb: string;
    }
    class Pos {
        start: number;
        end: number;
        prevStart: number;
        prevEnd: number;
        firstPart: string;
        lastPart: string;
        allowedToEnter: number;
    }
    interface Props {
        type?: string,
        date: Date | string | null,
        mandatory?: boolean,
        disabled?: boolean
    }

    const props = withDefaults(defineProps<Props>(), { type: 'date', date: null, mandatory: false, disabled: false });
    const emit = defineEmits<{ (e: 'set', date: string | null): void }>();
    const format: Format = {
        'screen': props.type == 'date-time' ? 'DD/MM/YYYY HH:mm' : 'DD/MM/YYYY',
        'server': props.type == 'date-time' ? 'YYYY-MM-DDTHH:mm:ss' : 'YYYY-MM-DD',
        'todb': 'YYYY-MM-DDTHH:mm:ss'
    };
    const date = ref(props.date ? (props.date instanceof Date) ? moment(props.date).format(format.screen) : props.date : null);
    const datepickerId = 'dtpck' + Math.floor(Math.random() * 999999999);
    const placeholder = props.type == 'date-time' ? 'dd/mm/yyyy hh:mm' : 'dd/mm/yyyy';
    const pickerType = props.type == 'date-time' ? 'datetime-local' : 'date';
    const onscreenDateVal = ref();

    const screenToServer = (inpDate: any, external: string | null) => {
        if (!inpDate && !external) { emit('set', null); onScreenDate(); return; }
        if ((inpDate && inpDate.target.value == placeholder) || (external && external == placeholder && !props.mandatory)) { if (!props.mandatory) { emit('set', null); date.value = null; } onScreenDate(); return; }
        const input = external ? external : inpDate.target.value;
        if (isNaN(input.replace(/[/:\s]/g, ''))) { onScreenDate(); return; }
        if (isNaN(moment(input as any, format.screen).format('x') as any) && !props.mandatory) { onScreenDate(); return; }
        date.value = moment(input, format.screen).format(format.todb);
        emit('set', date.value);
        onScreenDate()
    }

    const onScreenDate = (chrDate:any = null) => {
        onscreenDateVal.value = null;
        if ((!date.value || date.value == '') && !chrDate) { date.value = null; onscreenDateVal.value = ''; return; }
        onscreenDateVal.value = moment((chrDate == null ? date.value : chrDate.target.value), format.server).format(format.screen);
        if(chrDate) screenToServer(null,onscreenDateVal.value)
    }

    watch(() => props.date, (first: any, second: any) => {
        if (first != second) {
            date.value = first;
            onScreenDate();
        }
    });

    const applyDataMask = (el:HTMLInputElement) => {
        const elLength = placeholder.length, patternArray = [...placeholder],
        format = (e: any): any => {
            const pos = {
                start: el.selectionStart ?? 0,
                end: el.selectionEnd ?? 0,
                prevStart: parseInt(sessionStorage.getItem("selectionStart") ?? (el != null ? el.selectionStart != null ? el.selectionStart.toString() : '0' : '0')),
                prevEnd: parseInt(sessionStorage.getItem("selectionEnd") ?? (el != null ? el.selectionEnd != null ? el.selectionEnd.toString() : '0' : '0')),
                firstPart: el.value.slice(0, (el.selectionStart ?? 0)),
                lastPart: el.value.substring(el.value.length - (elLength - (parseInt(sessionStorage.getItem("selectionEnd") ?? (el != null ? el.selectionEnd != null ? el.selectionEnd.toString() : '0' : '0'))))),
                allowedToEnter: (elLength - el.value.length) < 1 ? 0 : (elLength - el.value.length)
            }
            if (e.inputType === "deleteContentBackward") {
                sessionStorage.removeItem("SingleDeleteCount")
                if (['/', ' ', ':'].includes(el.value.charAt(pos.start - 1))) {
                    el.setSelectionRange(pos.start, pos.end)
                }
                handleInputOrDelete(el, pos);
                el.setSelectionRange(pos.start, pos.start)
            } else if (e.inputType === "deleteContentForward") {
                if (pos.prevStart == pos.prevEnd) {
                    const delCount = sessionStorage.getItem("SingleDeleteCount") ? (parseInt(sessionStorage.getItem("SingleDeleteCount") ?? '1') + 1) : 1
                    pos.lastPart = pos.lastPart.slice(delCount - (pos.start == 0 ? 1 : 0));
                    pos.prevEnd = pos.prevEnd + delCount;
                    sessionStorage.setItem("SingleDeleteCount", delCount.toString());
                }
                handleInputOrDelete(el, pos);
                el.setSelectionRange(pos.start, pos.end)
            } else {
                resetSingleDelete();
                if (pos.start <= elLength) {
                    if ((isNaN(el.value.charAt(pos.start) as any) && !['/', ' ', ':'].includes(el.value.charAt(pos.start))) || el.value.length > 9) {
                        el.value = el.value.slice(0, pos.start) + el.value.slice(pos.start + 1)
                    } else { handleInputOrDelete(el, pos); }
                    el.setSelectionRange(pos.start + ((['/', ' ', ':'].includes(el.value.charAt(pos.start)) ? 1 : 0)), (pos.start + (['/', ' ', ':'].includes(el.value.charAt(pos.start)) ? 1 : 0)))
                } else { el.value = el.value.substring(0, elLength) }
            }
            rememberPos();
        },
        handleInputOrDelete = (el: HTMLInputElement, pos: Pos) => {
            el.value = pos.firstPart + (pos.allowedToEnter > 0 ? fillFromPattern(pos.start, pos.prevEnd) : '') + pos.lastPart;
        },
        fillFromPattern = (start: number, end: number) => {
            var fill = '';
            for (var i = 0; i < (end - start); i++) { fill += patternArray[start + i]; }
            return fill;
        },
        setCursor = () => {
            window.setTimeout(() => { el.setSelectionRange(0, 2); }, 25);
        },
        rememberPos = () => {
            var start = el.selectionStart ?? 0, end = el.selectionEnd ?? 0;
            sessionStorage.setItem("selectionStart", start.toString());
            sessionStorage.setItem("selectionEnd", end.toString());
        },
        resetRememberedPos = () => {
            sessionStorage.removeItem("selectionStart")
            sessionStorage.removeItem("selectionEnd")
            resetSingleDelete();
        },
        resetSingleDelete = () => {
            sessionStorage.removeItem("SingleDeleteCount");
        },
        checkIfEmpty = () => {
            if (!el.value || el.value == '') el.value = placeholder;
            window.setTimeout(() => { el.setSelectionRange(0, 0); }, 25);
        },
        checkKeyAndNextChar = (e: KeyboardEvent) => {
            if ((e.key < '0' || e.key > '9') && e.key != "Backspace" && e.key != "Delete" && e.key != "Enter") {
                e.preventDefault();
                return;
            }

            if (e.key !== "Backspace") {
                const start = el.selectionStart ?? 0;
                const newStart = start + (['/', ' ', ':'].includes(el.value.charAt(start)) ? 1 : 0);
                el.setSelectionRange(newStart, newStart);
            }

            if (e.key == "Delete") {
                const delCount = sessionStorage.getItem("SingleDeleteCount") ?? null;
                const selStart = sessionStorage.getItem("selectionStart") ?? null;
                if (delCount && selStart) {
                    if (parseInt(delCount) + parseInt(selStart) == elLength) {
                        e.preventDefault();
                    }
                }
            }

            if (e.key == "Enter") {
                e.preventDefault();
                if (isNaN(el.value.replace(/[/:\s]/g, '') as any)) {
                    if (el.value == placeholder) { date.value = null; el.value = ''; resetRememberedPos(); }
                    el.blur();
                    screenToServer(null, date.value);
                    window.setTimeout(() => { el.focus(); }, 25);
                    return;
                } else {
                    screenToServer(null, el.value);
                }
            }
        }
        el.addEventListener("keydown", (e: KeyboardEvent) => checkKeyAndNextChar(e));
        el.addEventListener("input", (e) => format(e)); 
        el.addEventListener("focus", (e: FocusEvent) => !el.value ? checkIfEmpty() : setCursor());
        el.addEventListener("mousedown", (e: MouseEvent) => (el.selectionStart = 0, el.selectionEnd = 0, resetSingleDelete()));
        el.addEventListener("mouseup", rememberPos);
        el.addEventListener("select", rememberPos);
        el.addEventListener("blur", () => ((el.value === placeholder && (el.value = "")), resetRememberedPos()));
        el.addEventListener("dragstart", (e) => e.preventDefault())
        el.addEventListener("ondrop", (e) => e.preventDefault())
    }

    onMounted(() => {
        Array.prototype.forEach.call(document.body.querySelectorAll("#" + datepickerId), applyDataMask);
    })

    onScreenDate();
</script>

<template>
    <div class="tt-datepicker-wrapper">
        <input type="text" 
               :value="onscreenDateVal" 
               @change="screenToServer($event)" 
               :placeholder="placeholder" 
               :class="['form-control-tt-datepicker', (disabled ? '' : 'tt-date-picker')]"
               :id="datepickerId"
               :disabled="disabled"
               pattern="[0-9]"/>
        <input v-if="!disabled"
               :type="pickerType" 
               @change="onScreenDate($event)" 
               class="chrome-date non-arrow-operated"
               :disabled="disabled"
               onclick="this.showPicker()" />
    </div>
</template>
