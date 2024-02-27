import ApiRequester from './ApiRequester'

class LoginApi {
    constructor(private requester: ApiRequester) { }
    public async Login(email: string, password: string): Promise<void> {
        await this.requester.request("/identity/login", "POST", { useSessionCookies: true }, JSON.stringify({ email, password }))
    }
}

export default LoginApi
