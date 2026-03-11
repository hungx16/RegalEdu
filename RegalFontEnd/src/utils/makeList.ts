import { AllocationEventStatusLabels, ApplicationUserTypeLabels, ClassMethodLabels, ClassStatusLabels, EventCategoryLabels, FormatTypeLabels, LevelTypeLabels, TopicTypeLabels, WorkTypeLabels } from '@/types/index';
export function getEventCategoryOptions(t: any) {
    if (t === undefined) {
        return Object.entries(EventCategoryLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(EventCategoryLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
export function getFormatOptions(t: any) {
    if (t === undefined) {
        return Object.entries(FormatTypeLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(FormatTypeLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
export function getTopicOptions(t: any) {
    if (t === undefined) {
        return Object.entries(TopicTypeLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(TopicTypeLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}

export function getApplicationUserTypeOptions(t: any) {
    if (t === undefined) {
        return Object.entries(ApplicationUserTypeLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(ApplicationUserTypeLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
export function getWorkTypeOptions(t: any) {
    if (t === undefined) {
        return Object.entries(WorkTypeLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(WorkTypeLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
export function getLevelTypeOptions(t: any) {
    if (t === undefined) {
        return Object.entries(LevelTypeLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(LevelTypeLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
export function getClassStatusOptions(t: any) {
    if (t === undefined) {
        return Object.entries(ClassStatusLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(ClassStatusLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
export function getClassMethodOptions(t: any) {
    if (t === undefined) {
        return Object.entries(ClassMethodLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(ClassMethodLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
export function getAllocationEventStatusOptions(t: any) {
    if (t === undefined) {
        return Object.entries(AllocationEventStatusLabels).map(([value, key]) => ({
            value: Number(value),
            label: key,
        }));
    }
    return Object.entries(AllocationEventStatusLabels).map(([value, key]) => ({
        value: Number(value),
        label: t(key),
    }));
}
