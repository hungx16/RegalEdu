
export enum DayOfWeek {
    Monday = 1,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday,
}

export const DAY_OF_WEEK_OPTIONS = [
    { key: 'dayOfWeek.monday', value: DayOfWeek.Monday },
    { key: 'dayOfWeek.tuesday', value: DayOfWeek.Tuesday },
    { key: 'dayOfWeek.wednesday', value: DayOfWeek.Wednesday },
    { key: 'dayOfWeek.thursday', value: DayOfWeek.Thursday },
    { key: 'dayOfWeek.friday', value: DayOfWeek.Friday },
    { key: 'dayOfWeek.saturday', value: DayOfWeek.Saturday },
    { key: 'dayOfWeek.sunday', value: DayOfWeek.Sunday },
];
export const getDayOfWeekKey = (value: number) =>
    DAY_OF_WEEK_OPTIONS.find((o) => o.value === value)?.key || '';


