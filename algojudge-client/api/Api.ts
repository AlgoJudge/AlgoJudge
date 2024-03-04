import LoginApi from './LoginApi'
import ApiRequester from './ApiRequester'
import ActivityApi from './ActivityApi'
import EventDispatcher from './EventDispatcher'

interface Session {
    eventDispatcher: EventDispatcher,
    requester: ApiRequester,
    loginApi: LoginApi,
    activityApi: ActivityApi
}

class Api {
    static create(): Session {
        const eventDispatcher = new EventDispatcher();
        const requester = new ApiRequester(import.meta.env.VITE_APP_API_BASE_URL, eventDispatcher);
        const loginApi = new LoginApi(requester)
        const activityApi = new ActivityApi(requester);
        return {
            eventDispatcher,
            requester,
            loginApi,
            activityApi
        }
    }
}

export type { Session, EventDispatcher, ApiRequester, LoginApi, ActivityApi }
export default Api;