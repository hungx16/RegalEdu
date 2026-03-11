<template>
  <div class="app-container p-6">
    <!-- ====== KPI CARDS ====== -->
    <el-row :gutter="20" class="mb-6">
      <el-col :span="6" v-for="(card, index) in summaryCards" :key="index">
        <div class="card-kpi p-5 rounded-4 shadow-sm bg-white h-100">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <span class="fw-semibold text-muted">{{ card.title }}</span>
            <i :class="card.icon + ' text-primary fs-3'"></i>
          </div>
          <h2 class="fw-bold mb-1">{{ card.value }}</h2>
          <div class="text-success small d-flex align-items-center">
            <i class="bi bi-arrow-up-right me-1"></i>{{ card.change }}
          </div>
        </div>
      </el-col>
    </el-row>

    <!-- ====== CHARTS ====== -->
    <el-row :gutter="20">
      <!-- PIE CHART SECTION -->
      <el-col :span="12">
        <div class="card p-5 rounded-4 shadow-sm bg-white h-100">
          <h5 class="fw-semibold mb-1">Nguồn khách hàng tiềm năng</h5>
          <p class="text-muted small mb-4">Phân bổ theo kênh tiếp cận</p>

          <!-- Biểu đồ -->
          <div class="chart-wrapper mb-4">
            <canvas ref="pieChart"></canvas>
          </div>

          <!-- Chú thích -->
          <div class="legend-row d-flex flex-wrap gap-3 justify-content-start">
            <div v-for="item in legendItems" :key="item.label" class="d-flex align-items-center gap-2">
              <div class="legend-color rounded-circle" :style="{ backgroundColor: item.color }"></div>
              <span class="small fw-semibold" :style="{ color: item.color }">{{ item.label }}</span>
            </div>
          </div>
        </div>
      </el-col>


      <!-- Bar chart -->
      <el-col :span="12">
        <div class="card p-5 rounded-4 shadow-sm bg-white h-100">
          <h5 class="fw-semibold mb-4">Doanh thu theo chi nhánh</h5>
          <p class="text-muted small mb-2">Triệu đồng</p>
          <canvas ref="barChart"></canvas>
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Chart from 'chart.js/auto'

const summaryCards = ref([
  { title: 'Học viên', value: '2.845', change: '+12.5% so với tháng trước', icon: 'bi bi-mortarboard' },
  { title: 'Khách hàng tiềm năng', value: '1.560', change: '+8.2% so với tháng trước', icon: 'bi bi-person-lines-fill' },
  { title: 'Doanh số', value: '8.090.000.000', change: '+15.4% so với tháng trước', icon: 'bi bi-graph-up-arrow' },
  { title: 'Chi nhánh', value: '12', change: '+2 chi nhánh mới', icon: 'bi bi-house' }
])

const pieChart = ref<HTMLCanvasElement | null>(null)
const barChart = ref<HTMLCanvasElement | null>(null)

const legendItems = [
  { label: 'Event', color: '#f59f00' },
  { label: 'Facebook', color: '#0d6efd' },
  { label: 'Khác', color: '#adb5bd' },
  { label: 'Social', color: '#4dabf7' },
  { label: 'Support', color: '#2f9e44' },
  { label: 'Tiktok', color: '#000000' },
  { label: 'Website', color: '#6f42c1' },
  { label: 'Zalo', color: '#228be6' }
]

onMounted(() => {
  // === Pie Chart ===
  if (pieChart.value) {
    new Chart(pieChart.value, {
      type: 'pie',
      data: {
        labels: ['Website: 29%', 'Social: 21%', 'Event: 18%', 'Zalo: 12%', 'Facebook: 10%', 'Tiktok: 6%', 'Support: 3%', 'Khác: 2%'],
        datasets: [{
          data: [29, 21, 18, 12, 10, 6, 3, 2],
          backgroundColor: legendItems.map(i => i.color),
          borderWidth: 0
        }]
      },
      options: {
        plugins: {
          legend: { display: false },
          tooltip: { enabled: true }
        }
      }
    })
  }

  // === Bar Chart ===
  if (barChart.value) {
    new Chart(barChart.value, {
      type: 'bar',
      data: {
        labels: ['RE Kiến An', 'RE Thủy Nguyên', 'RE Đông Anh', 'RE Hải An', 'RE Vạn Định', 'RE Vạn Khê', 'RE Phúc Lâm', 'RE Hải Phòng'],
        datasets: [{
          label: 'Doanh thu (triệu đồng)',
          data: [1700, 2100, 1000, 800, 700, 600, 500, 450],
          backgroundColor: '#6f42c1',
          borderRadius: 6
        }]
      },
      options: {
        scales: {
          y: { beginAtZero: true }
        },
        plugins: {
          legend: { display: false }
        }
      }
    })
  }
})
</script>


<style scoped>
.card-kpi {
  transition: 0.3s;
  border: 1px solid #f1f1f1;
}

.card-kpi:hover {
  transform: translateY(-3px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.05);
}

.chart-wrapper {
  width: 100%;
  max-width: 320px;
  margin: 0 auto;
}

.legend-row {
  border-top: 1px solid #f1f1f1;
  padding-top: 0.75rem;
}

.legend-color {
  width: 12px;
  height: 12px;
}
</style>
