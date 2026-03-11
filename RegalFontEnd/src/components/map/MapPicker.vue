<template>
    <div class="map-picker">
        <div class="toolbar">
            <el-button size="small" @click="locateMe" :loading="locating">
                <el-icon>
                    <Aim />
                </el-icon>
                <span>Lấy vị trí của tôi</span>
            </el-button>
            <div class="coords">
                <span>Lat: <strong>{{ fmt(lat) }}</strong></span>
                <span class="sep">|</span>
                <span>Lng: <strong>{{ fmt(lng) }}</strong></span>
                <el-button size="small" link type="danger" @click="clearPos" :disabled="lat == null || lng == null">
                    Xóa tọa độ
                </el-button>
            </div>
        </div>

        <div ref="mapEl" class="leaflet-map"></div>

        <div class="hint">
            <el-icon>
                <Pointer />
            </el-icon>
            <span>Nhấp vào bản đồ để chọn vị trí.</span>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, onBeforeUnmount, ref, watch, nextTick, defineExpose } from 'vue'
import { Aim, Pointer } from '@element-plus/icons-vue'
import 'leaflet/dist/leaflet.css'
import L from 'leaflet'

// ✅ Import ảnh icon tĩnh để Vite build đúng URL
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png'
import markerIcon from 'leaflet/dist/images/marker-icon.png'
import markerShadow from 'leaflet/dist/images/marker-shadow.png'
import { ElMessage } from 'element-plus'

// Fix default icon
delete (L.Icon.Default.prototype as any)._getIconUrl
L.Icon.Default.mergeOptions({
    iconRetinaUrl: markerIcon2x,
    iconUrl: markerIcon,
    shadowUrl: markerShadow
})

/**
 * v-model:lat & v-model:lng
 * Sử dụng:
 * <MapPicker v-model:lat="formData.latitude" v-model:lng="formData.longitude" />
 */
const props = defineProps<{
    lat?: number | null
    lng?: number | null
    // Tùy chọn
    height?: string // vd: '360px'
    zoom?: number
}>()

const emit = defineEmits<{
    (e: 'update:lat', v: number | null): void
    (e: 'update:lng', v: number | null): void
}>()

const mapEl = ref<HTMLDivElement | null>(null)
let map: L.Map | null = null
let marker: L.Marker | null = null

const locating = ref(false)

// Việt Nam (Hà Nội) mặc định
const DEFAULT_CENTER: L.LatLngExpression = [21.0278, 105.8342]
const DEFAULT_ZOOM = props.zoom ?? 5

const fmt = (v: number | null | undefined) => (v == null ? '—' : v.toFixed(6))

function setMarker(lat: number, lng: number, pan = false) {
    if (!map) return
    const latLng = L.latLng(lat, lng)
    if (!marker) {
        marker = L.marker(latLng, { draggable: true }).addTo(map)
        marker.on('dragend', () => {
            const p = marker!.getLatLng()
            emit('update:lat', +p.lat.toFixed(6))
            emit('update:lng', +p.lng.toFixed(6))
        })
    } else {
        marker.setLatLng(latLng)
    }
    if (pan) map.setView(latLng, 16)
}

function clearPos() {
    emit('update:lat', null)
    emit('update:lng', null)
    if (marker && map) {
        map.removeLayer(marker)
        marker = null
    }
}

async function locateMe() {
    if (!map) return
    if (!('geolocation' in navigator)) {
        ElMessage?.warning?.('Trình duyệt không hỗ trợ xác định vị trí.')
        return
    }
    locating.value = true
    navigator.geolocation.getCurrentPosition(
        (pos) => {
            locating.value = false
            const { latitude, longitude } = pos.coords
            emit('update:lat', +latitude.toFixed(6))
            emit('update:lng', +longitude.toFixed(6))
            setMarker(latitude, longitude, true)
        },
        (err) => {
            locating.value = false
            console.error(err)
            ElMessage?.error?.('Không lấy được vị trí. Hãy kiểm tra quyền truy cập vị trí.')
        },
        { enableHighAccuracy: true, timeout: 10000, maximumAge: 0 }
    )
}

onMounted(async () => {
    await nextTick()
    if (!mapEl.value) return

    map = L.map(mapEl.value, {
        center: DEFAULT_CENTER,
        zoom: DEFAULT_ZOOM,
        zoomControl: true
    })

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        maxZoom: 19,
        attribution:
            '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
    }).addTo(map)

    // Set marker nếu đã có lat/lng
    if (props.lat != null && props.lng != null) {
        setMarker(props.lat, props.lng, true)
    }

    // Click để đặt marker
    map.on('click', (e: L.LeafletMouseEvent) => {
        const { lat, lng } = e.latlng
        emit('update:lat', +lat.toFixed(6))
        emit('update:lng', +lng.toFixed(6))
        setMarker(lat, lng, false)
    })

    // Nếu map nằm trong dialog → delay invalidateSize
    setTimeout(() => {
        map?.invalidateSize()
    }, 250)
})

onBeforeUnmount(() => {
    if (map) {
        map.remove()
        map = null
    }
})

// Đồng bộ khi lat/lng ngoài thay đổi
watch(
    () => [props.lat, props.lng] as const,
    ([lat, lng]) => {
        if (!map) return
        if (lat != null && lng != null) {
            setMarker(lat, lng, false)
        } else if (marker) {
            map.removeLayer(marker)
            marker = null
        }
    }
)

// Cho phép parent gọi resize() khi dialog mở ra
function resize() {
    map?.invalidateSize()
}
defineExpose({ resize })
</script>

<style scoped>
.map-picker {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.toolbar {
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
}

.coords {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 13px;
    color: #475569;
}

.coords .sep {
    color: #cbd5e1;
}

.leaflet-map {
    width: 100%;
    height: v-bind(heightVar);
    border-radius: 12px;
    overflow: hidden;
    border: 1px solid #e6eaf0;
    box-shadow: 0 4px 12px rgba(14, 30, 62, 0.06);
}

.hint {
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 12px;
    color: #6b7280;
}

/* Dark mode nhẹ */
:global(html.dark) .leaflet-map {
    border-color: #2c2c2c;
    box-shadow: none;
}
</style>

<script lang="ts">
// 👇 work-around để dùng prop height trong CSS (v-bind)
export default {
    computed: {
        heightVar(): string {
            // @ts-ignore
            return this.$props?.height ?? '360px'
        }
    }
}
</script>
