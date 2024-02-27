import LoginApi from './LoginApi'
import ApiRequester from './ApiRequester'
import ActivityApi from './ActivityApi'

const apiRequester = new ApiRequester("https://localhost:7004");
const loginApi = new LoginApi(apiRequester)
const activityApi = new ActivityApi(apiRequester);

export { loginApi, activityApi }