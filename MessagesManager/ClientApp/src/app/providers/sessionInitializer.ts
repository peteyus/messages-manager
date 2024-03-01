export function initializeSession() {
    return () => localStorage.setItem('sessionId', crypto.randomUUID());
}