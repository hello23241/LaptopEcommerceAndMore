// wwwroot/js/site.js
document.addEventListener('DOMContentLoaded', function () {

    // --- 1. PASSWORD VALIDATION ON REGISTER MODAL ---
    const modalPassInput = document.getElementById('modalPasswordInput');
    if (modalPassInput) {
        modalPassInput.addEventListener('input', function () {
            const val = this.value;
            const requirements = [
                { el: document.getElementById('m-req-length'), valid: val.length >= 8, text: "At least 8 characters" },
                { el: document.getElementById('m-req-upper'), valid: /[A-Z]/.test(val), text: "1 capital letter (A-Z)" },
                { el: document.getElementById('m-req-number'), valid: /[0-9]/.test(val), text: "1 digit (0-9)" },
                { el: document.getElementById('m-req-special'), valid: /[!@#$%^&*(),.?":{}|<>]/.test(val), text: "1 special character" }
            ];

            requirements.forEach(req => {
                if (req.el) {
                    if (req.valid) {
                        req.el.classList.replace('text-danger', 'text-success');
                        req.el.innerHTML = '✔ ' + req.text;
                    } else {
                        req.el.classList.replace('text-success', 'text-danger');
                        req.el.innerHTML = '✘ ' + req.text;
                    }
                }
            });
        });
    }

    // --- THÊM MỚI 1B. REAL-TIME PASSWORD MATCHING FOR REGISTER MODAL ---
    const confirmPasswordInput = document.getElementById('modalConfirmPassword');
    const matchMessage = document.getElementById('passwordMatchMessage');
    const registerSubmitBtn = document.getElementById('btnRegisterSubmit');

    if (confirmPasswordInput && modalPassInput) {
        function checkPasswordMatch() {
            const pwd = modalPassInput.value;
            const confirmPwd = confirmPasswordInput.value;

            // Nếu ô nhập lại trống, đưa UI về trạng thái ẩn nguyên bản
            if (confirmPwd === "") {
                matchMessage.textContent = "";
                matchMessage.style.backgroundColor = "transparent";
                matchMessage.style.borderColor = "transparent";
                if (registerSubmitBtn) registerSubmitBtn.disabled = false;
                confirmPasswordInput.style.borderColor = "#ced4da";
                return;
            }

            if (pwd === confirmPwd) {
                matchMessage.textContent = "✔ Passwords match perfectly!";
                matchMessage.style.color = "#155724";
                matchMessage.style.backgroundColor = "#d4edda";
                matchMessage.style.borderColor = "#c3e6cb";
                confirmPasswordInput.style.borderColor = "#28a745";
                if (registerSubmitBtn) registerSubmitBtn.disabled = false; // Mở khóa nút REGISTER
            } else {
                matchMessage.textContent = "✘ Passwords do not match.";
                matchMessage.style.color = "#721c24";
                matchMessage.style.backgroundColor = "#f8d7da";
                matchMessage.style.borderColor = "#f5c6cb";
                confirmPasswordInput.style.borderColor = "#dc3545";
                if (registerSubmitBtn) registerSubmitBtn.disabled = true;  // Khóa nút không cho submit
            }
        }

        // Theo dõi liên tục sự kiện nhập liệu của cả 2 ô mật khẩu
        confirmPasswordInput.addEventListener('input', checkPasswordMatch);
        modalPassInput.addEventListener('input', checkPasswordMatch);
    }

    // --- 2. AUTO-CLOSE ALERTS AFTER 5 SECONDS ---
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            const bsAlert = bootstrap.Alert.getInstance(alert) || new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // --- 3. FIX OVERLAPPING MODALS (Switching between Login - Register - Verify) ---
    document.querySelectorAll('[data-bs-toggle="modal"]').forEach(btn => {
        btn.addEventListener('click', function () {
            const target = this.getAttribute('data-bs-target');
            const currentModal = this.closest('.modal');
            if (currentModal && target) {
                const modalInst = bootstrap.Modal.getInstance(currentModal);
                if (modalInst) modalInst.hide();
            }
        });
    });

    // --- 4. OTP HANDLING FOR VERIFY EMAIL MODAL ---
    const otpFields = document.querySelectorAll('#verifyEmailModal .otp-field');
    const modalHiddenOtp = document.getElementById('modalHiddenOtp');

    if (otpFields.length > 0) {
        otpFields.forEach((field, index) => {
            field.addEventListener('input', (e) => {
                if (e.target.value.length >= 1 && index < otpFields.length - 1) {
                    otpFields[index + 1].focus();
                }
                combineModalOtp();
            });

            field.addEventListener('keydown', (e) => {
                if (e.key === 'Backspace' && field.value === '' && index > 0) {
                    otpFields[index - 1].focus();
                }
            });

            field.addEventListener('paste', (e) => {
                const data = e.clipboardData.getData('text').trim();
                if (data.length === otpFields.length && /^\d+$/.test(data)) {
                    data.split('').forEach((char, i) => {
                        otpFields[i].value = char;
                    });
                    combineModalOtp();
                    otpFields[otpFields.length - 1].focus();
                }
                e.preventDefault();
            });
        });
    }

    function combineModalOtp() {
        if (modalHiddenOtp) {
            let otp = "";
            otpFields.forEach(f => otp += f.value);
            modalHiddenOtp.value = otp;
        }
    }

    // --- 5. AUTOMATIC MODAL SWITCHING (Server-side Trigger) ---
    const triggerInput = document.getElementById('triggerVerifyModal');
    if (triggerInput && triggerInput.value === 'true') {
        const userEmail = document.getElementById('tempUserEmail')?.value;

        const displayEmail = document.getElementById('displayEmail');
        const hiddenEmailFields = document.querySelectorAll('.modalVerifyEmail, #modalVerifyEmail');

        if (displayEmail) displayEmail.innerText = userEmail;
        hiddenEmailFields.forEach(input => input.value = userEmail);

        const regModalEl = document.getElementById('registerModal');
        if (regModalEl) {
            const regInstance = bootstrap.Modal.getOrCreateInstance(regModalEl);
            regInstance.hide();
        }

        setTimeout(() => {
            const verifyModalEl = document.getElementById('verifyEmailModal');
            if (verifyModalEl) {
                const verifyInstance = new bootstrap.Modal(verifyModalEl);
                verifyInstance.show();
            }
        }, 400);
    }
});