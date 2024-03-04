enum EventType {
    UNAUTHORIZED = "ajUnauthorized",
    FORBIDDEN = "ajForbidden",
    INVALID_STATUS_CODE = "ajInvalidStatusCode"
}

class EventDispatcher extends EventTarget {
    public dispatch(type: EventType): void {
        this.dispatchEvent(new CustomEvent(type));
    }
}

export { EventType }
export default EventDispatcher;