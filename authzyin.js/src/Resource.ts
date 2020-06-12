// eslint-disable-next-line @typescript-eslint/no-empty-interface
export interface Resource {}

export interface ConstantWrapperResource {
    // we only allow number or string here for now
    value: number | string;
}
