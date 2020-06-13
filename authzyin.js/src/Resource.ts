// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface Resource {}

export interface ConstantWrapperResource extends Resource {
    // we only allow number or string here for now
    value: number | string;
}
