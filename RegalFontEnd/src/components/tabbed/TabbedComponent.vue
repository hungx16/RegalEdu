<template>
    <div class="tabbed-wrapper">
        <div class="custom-tabs">
            <button type="button" v-for="tab in tabs" :key="tab.name"
                :class="['tab-btn', { active: tab.name === modelValue }]" @click="onTabClick(tab.name)">
                {{ tab.label }}
            </button>
        </div>

        <div class="tab-content">
            <slot :name="modelValue" />
        </div>
    </div>
</template>


<script setup lang="ts">

const props = defineProps<{
    tabs: { name: string; label: string }[];
    modelValue: string;
}>();

const emit = defineEmits(['update:modelValue']);

function onTabClick(name: string) {
    if (name !== props.modelValue) emit('update:modelValue', name);
}
</script>

<style scoped>
.custom-tabs {
    display: flex;
    background: #f7f7f9;
    border-radius: 20px;
    padding: 2px;
    width: 100%;
    border: 1px solid #ececec;
    margin-bottom: 1.2rem;
    box-sizing: border-box;
    min-height: 36px;
    /* Giảm chiều cao */
}

.tab-btn {
    flex: 1 1 0;
    border: none;
    outline: none;
    background: transparent;
    color: #222;
    font-size: 15px;
    padding: 7px 0;
    /* Giảm chiều cao */
    border-radius: 18px;
    /* Giảm bo góc để hợp với chiều cao */
    font-weight: 500;
    transition: background 0.25s, color 0.18s;
    cursor: pointer;
    min-width: 0;
}

.tab-btn.active {
    background: #fff;
    color: #222;
    box-shadow: 0 1px 4px rgba(0, 0, 0, 0.02);
}

.tab-btn:not(.active) {
    background: #f5f5f7;
    color: #555;
}

.tab-btn:disabled {
    opacity: 0.6;
    cursor: not-allowed;
}

.tab-content {
    padding: 1.2rem 0 0 0;
}
</style>
