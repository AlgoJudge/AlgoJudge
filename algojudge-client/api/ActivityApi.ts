import ApiRequester from './ApiRequester'

interface ActivityInfo {
    id: number,
    shortName: string,
    name: string,
    type: string,
    startDate: string,
    endDate: string
}

interface ActivityCreateModel {
    shortName: string,
    name: string,
    type: string,
    startDate: string,
    endDate: string
}

class ActivityApi {
    constructor(private requester: ApiRequester) { }

    public async getList(): Promise<ActivityInfo[]> {
        return await this.requester.request("/activity/list", "GET", undefined);
    }

    public async create(model: ActivityCreateModel): Promise<ActivityInfo> {
        return await this.requester.request("/activity/create", "POST", undefined, JSON.stringify(model));
    }
}

export type { ActivityInfo, ActivityCreateModel }

export default ActivityApi
