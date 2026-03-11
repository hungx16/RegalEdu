<template>
  <div class="app-container p-6">
    <!-- ===== HEADER ===== -->
    <div class="card border-0 shadow-sm rounded-4 p-6 bg-gradient-to-r from-purple-100 to-blue-100 mb-8">
      <div class="d-flex justify-content-between align-items-start flex-wrap gap-3">
        <div>
          <h1 class="fw-bold fs-2 text-dark mb-2">Bee AI</h1>
          <p class="text-muted mb-0" style="max-width: 650px;">
            Trợ lý AI thông minh cho giáo dục. <br />
            Nền tảng AI giúp tự động hóa quy trình tuyển sinh, tư vấn và chăm sóc khách hàng,
            tối ưu hóa hiệu quả làm việc và tỷ lệ chuyển đổi thông qua phân tích thông minh.
          </p>
        </div>
        <el-button type="primary" class="rounded-pill px-5 py-3 fw-semibold shadow-sm">
          <i class="bi bi-stars me-2"></i> Khám phá AI
        </el-button>
      </div>
    </div>

    <!-- ===== FEATURE CARDS ===== -->
    <el-row :gutter="20">
      <el-col v-for="(feature, index) in features" :key="index" :span="8" class="mb-5">
        <div class="card feature-card h-100 p-5 rounded-4 shadow-sm" :style="{ borderColor: feature.color }">
          <div class="d-flex align-items-center gap-3 mb-4">
            <div class="feature-icon rounded-3 d-flex align-items-center justify-content-center"
              :style="{ backgroundColor: feature.bgColor, color: feature.color }">
              <i :class="feature.icon + ' fs-3'"></i>
            </div>
            <h5 class="fw-semibold mb-0">{{ feature.title }}</h5>
          </div>
          <p class="text-muted mb-4">{{ feature.desc }}</p>

          <div class="d-flex justify-content-between align-items-center">
            <a v-if="feature.status === 'active'" href="javascript:void(0)" @click="openChat(feature)"
              class="text-primary fw-semibold text-decoration-none">
              Khám phá <i class="bi bi-arrow-right-short"></i>
            </a>
            <span v-else class="text-muted fw-semibold">
              Sắp ra mắt <i class="bi bi-star ms-1"></i>
            </span>
          </div>
        </div>
      </el-col>
    </el-row>

    <!-- ===== BEE AI CHAT DRAWER ===== -->
    <el-drawer v-model="drawerVisible" size="480px" direction="rtl" class="bee-ai-drawer" :with-header="false">
      <div class="p-5 h-100 d-flex flex-column">
        <!-- Header -->
        <div class="d-flex justify-content-between align-items-start mb-4">
          <div>
            <h4 class="fw-bold mb-1">{{ selectedFeature?.title }}</h4>
            <p class="text-muted small mb-0">{{ selectedFeature?.desc }}</p>
          </div>
          <el-button class="el-button-normal" type="text" @click="drawerVisible = false">
            <i class="bi bi-x-lg fs-5"></i>
          </el-button>
        </div>

        <!-- Chat content -->
        <div ref="chatBody" class="chat-body flex-grow-1 bg-light rounded-4 p-4 mb-4 overflow-auto">
          <div v-for="(msg, idx) in chatMessages" :key="idx" class="mb-3">
            <div v-if="msg.sender === 'bot'" class="text-start">
              <div class="fw-semibold text-primary mb-1">🤖 Bee AI</div>
              <div class="bg-white border rounded-3 p-3 shadow-sm small w-auto d-inline-block">
                {{ msg.text }}
              </div>
            </div>
            <div v-else class="text-end">
              <div class="bg-primary text-white rounded-3 p-3 d-inline-block small">
                {{ msg.text }}
              </div>
            </div>
          </div>
        </div>

        <!-- Chat input -->
        <div class="d-flex align-items-center">
          <el-input v-model="message" placeholder="Nhập câu hỏi của bạn..." class="flex-grow-1 me-3"
            @keyup.enter="sendMessage" />
          <el-button type="primary" @click="sendMessage">
            <i class="bi bi-send"></i>
          </el-button>
        </div>

      </div>
    </el-drawer>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue'

const drawerVisible = ref(false)
const selectedFeature = ref<any>(null)
const message = ref('')

// Danh sách tin nhắn lưu trong bộ nhớ tạm
interface ChatMessage {
  sender: 'user' | 'bot'
  text: string
}

const chatMessages = ref<ChatMessage[]>([])

const openChat = (feature: any) => {
  selectedFeature.value = feature
  drawerVisible.value = true
  chatMessages.value = [
    {
      sender: 'bot',
      text: `Xin chào! Tôi là Bee AI, trợ lý thông minh của bạn. Tôi sẽ hỗ trợ bạn về ${feature.title}. Bạn cần giúp gì hôm nay?`
    }
  ]
}

const sendMessage = async () => {
  const userInput = message.value.trim()
  if (!userInput) return

  // Thêm tin nhắn người dùng
  chatMessages.value.push({ sender: 'user', text: userInput })
  message.value = ''

  // Đợi Vue render tin nhắn người dùng rồi scroll xuống cuối
  await nextTick()
  scrollToBottom()

  // Giả lập phản hồi từ hệ thống
  setTimeout(async () => {
    chatMessages.value.push({
      sender: 'bot',
      text: '💡 Hệ thống Bee AI hiện đang trong quá trình xây dựng. Vui lòng quay lại sau.'
    })
    await nextTick()
    scrollToBottom()
  }, 800)
}

const chatBody = ref<HTMLElement | null>(null)
const scrollToBottom = () => {
  if (chatBody.value) {
    chatBody.value.scrollTop = chatBody.value.scrollHeight
  }
}

const features = ref([
  {
    title: 'AI hỗ trợ đánh giá và phân loại khách hàng',
    desc: 'Tự động đánh giá và phân loại khách hàng tiềm năng dựa trên dữ liệu hành vi và thông tin cá nhân.',
    icon: 'bi bi-person-vcard',
    color: '#a855f7',
    bgColor: '#f3e8ff',
    status: 'active'
  },
  {
    title: 'AI chấm điểm khách hàng tiềm năng',
    desc: 'Hệ thống chấm điểm thông minh giúp ưu tiên khách hàng có khả năng chuyển đổi cao.',
    icon: 'bi bi-star',
    color: '#facc15',
    bgColor: '#fef9c3',
    status: 'active'
  },
  {
    title: 'AI hỗ trợ kịch bản tư vấn khóa học/xử lý từ chối',
    desc: 'Đề xuất kịch bản tư vấn phù hợp và cách xử lý khi khách hàng từ chối.',
    icon: 'bi bi-chat-square-text',
    color: '#60a5fa',
    bgColor: '#dbeafe',
    status: 'active'
  },
  {
    title: 'AI phân tích phản hồi khách hàng',
    desc: 'Phân tích và tổng hợp phản hồi từ khách hàng để cải thiện dịch vụ.',
    icon: 'bi bi-bar-chart',
    color: '#22c55e',
    bgColor: '#dcfce7',
    status: 'active'
  },
  {
    title: 'AI hỗ trợ kịch bản chăm sóc khách hàng',
    desc: 'Tự động hóa quy trình chăm sóc khách hàng với kịch bản được cá nhân hóa.',
    icon: 'bi bi-heart',
    color: '#ec4899',
    bgColor: '#fce7f3',
    status: 'coming'
  },
  {
    title: 'AI hỗ trợ chăm sóc khách hàng sau bán hàng',
    desc: 'Duy trì mối quan hệ và tăng giá trị khách hàng sau khi đăng ký học.',
    icon: 'bi bi-heart-pulse',
    color: '#8b5cf6',
    bgColor: '#ede9fe',
    status: 'coming'
  }
])
</script>


<style scoped>
.feature-card {
  border: 1.5px solid transparent;
  transition: all 0.25s ease;
}

.feature-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.05);
}

.feature-icon {
  width: 44px;
  height: 44px;
}

.chat-body {
  min-height: 250px;
}

.el-button-normal {
  color: cadetblue !important;
}
</style>
