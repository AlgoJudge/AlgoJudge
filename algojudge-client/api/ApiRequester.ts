class ApiRequester {
    constructor(private baseUrl: string) { }

    public async request(path: string, method: "GET" | "POST", query: any = {}, body: any = undefined): Promise<any> {
        const url = this.baseUrl + path + "?" + new URLSearchParams(query);
        const res = await fetch(url, {
            method,
            body,
            headers: new Headers({ 'content-type': 'application/json' }),
            credentials: "include"
        });
        if (res.status == 401) {
            throw new Error("Unauthorized");
        } else if (res.status == 403) {
            throw new Error("Forbidden");
        } else if (res.status != 200) {
            throw new Error("Invalid status");
        }
        try {
            return await res.json();
        } catch (e) { /* empty */ }
        return "{}";
    }
}

export default ApiRequester;