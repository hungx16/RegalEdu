import { ref } from 'vue'

export function useLoadingWithTimeout(timeoutMs = 10000) {
    const loading = ref(false)
    let timer: number | undefined

    function start() {
        loading.value = true
        clearTimeout(timer)
        timer = window.setTimeout(() => {
            loading.value = false
        }, timeoutMs)
    }
    function stop() {
        loading.value = false
        clearTimeout(timer)
    }
    // Cleanup on unmount
    return {
        loading,
        start,
        stop
    }
}
