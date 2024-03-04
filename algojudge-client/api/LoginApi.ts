import ApiRequester from './ApiRequester'

interface Info {
    email: string
}

class LoginApi {
    constructor(private requester: ApiRequester) { }

    public async login(email: string, password: string): Promise<void> {
        await this.requester.request("/identity/login", "POST", { useSessionCookies: true }, JSON.stringify({ email, password }))
    }

    public async register(email: string, password: string): Promise<void> {
        await this.requester.request("/identity/register", "POST", undefined, JSON.stringify({ email, password }));
    }

    public async getInfo(): Promise<Info> {
        return await this.requester.request('/identity/manage/info', "GET");
    }
}

export type { Info }
export default LoginApi;
