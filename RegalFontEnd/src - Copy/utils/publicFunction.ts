export function buildSelectOptions<T>(
    list: T[],
    selectedId: any,
    getLabel: (item: T) => string,
    getDisabled?: (item: T) => boolean
) {
    const selected = list.find(e => (e as any).id === selectedId);

    let options = list.map(e => ({
        label: getLabel(e),
        value: (e as any).id,
        disabled: getDisabled ? getDisabled(e) : false
    }));

    // Đảm bảo option đang chọn (có thể là inactive) luôn hiển thị đầu danh sách
    if (selected && !options.some(opt => opt.value === selectedId)) {
        options.unshift({
            label: getLabel(selected),
            value: selectedId,
            disabled: getDisabled ? getDisabled(selected) : false
        });
    }

    // Trả về options và option đang chọn (nếu có)
    return {
        options,
        selectedOption: options.find(opt => opt.value === selectedId) || null
    };
}
export function getFileName(fileName: string): string {
    const parts = fileName.split('/');
    return parts.length > 1 ? parts[parts.length - 1] : '';
}

// Regex: ≥8 ký tự, có ít nhất 1 chữ thường, 1 chữ HOA, 1 số và 1 ký tự đặc biệt
// ≥8 ký tự, có ít nhất 1 chữ thường, 1 chữ HOA, 1 số, 1 ký tự đặc biệt
export const strongPw = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$/;

/** 
 * Kiểm tra mật khẩu mạnh.
 * @param v        mật khẩu
 * @param options  allowEmpty: true => rỗng vẫn coi là hợp lệ (tuỳ ngữ cảnh)
 */
export function isStrongPassword(
    v: string,
    options: { allowEmpty?: boolean } = {}
): boolean {
    const { allowEmpty = false } = options;
    if (!v) return !!allowEmpty;
    return strongPw.test(v);
}
