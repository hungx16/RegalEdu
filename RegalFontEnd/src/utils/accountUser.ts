import { getAssetPath } from "@/core/helpers/assets"

export interface UserLS {

    userName?: string            // login email hiện tại của anh
    originalUserName?: string    // full name hiển thị
    companyEmail?: string
    personalEmail?: string
    avatarUrl?: string           // base64 không có header (đúng như ảnh anh gửi)
    roles?: string[]
    accessToken?: string
    refreshToken?: string
    succeeded?: boolean
    userStatus?: number
}
/** ---- Helpers đọc localStorage.userData an toàn ---- */
export function readUserFromLS(): UserLS | null {
    try {
        const raw = localStorage.getItem('userData')
        if (!raw) return null
        return JSON.parse(raw) as UserLS
    } catch {
        return null
    }
}
export function makeAvatarSrc(val?: string) {
    if (!val) return getAssetPath('media/avatars/avatar.png')
    if (val.startsWith('http') || val.startsWith('data:')) return val
    // đoán định dạng: iVBOR... -> png, /9j/ -> jpeg
    const mime = val.startsWith('iVBOR') ? 'image/png' : 'image/jpeg'
    return `data:${mime};base64,${val}`
}