import EventDispatcher, { EventType } from "./EventDispatcher";

class UnauthorizedError extends Error {
    constructor() {
        super("Unauthorized")
    }
}
class ForbiddenError extends Error {
    constructor() {
        super("Forbidden")
    }
}
class InvalidStatusError extends Error {
    constructor() {
        super("Invalid status")
    }
}

class ApiRequester {
    constructor(private baseUrl: string, private eventDispatcher: EventDispatcher) { }

    public async request(path: string, method: "GET" | "POST", query: any = {}, body: any = undefined): Promise<any> {
        const url = this.baseUrl + path + "?" + new URLSearchParams(query);
        const res = await fetch(url, {
            method,
            body,
            headers: new Headers({ 'content-type': 'application/json' }),
            credentials: "include"
        });
        if (res.status == 401) {
            this.eventDispatcher.dispatch(EventType.UNAUTHORIZED);
            throw new UnauthorizedError();
        } else if (res.status == 403) {
            this.eventDispatcher.dispatch(EventType.FORBIDDEN);
            throw new ForbiddenError();
        } else if (res.status != 200) {
            this.eventDispatcher.dispatch(EventType.INVALID_STATUS_CODE);
            throw new InvalidStatusError();
        }
        try {
            return await res.json();
        } catch (e) { /* empty */ }
        return "{}";
    }
}

export { UnauthorizedError }
export default ApiRequester;