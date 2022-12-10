declare namespace app.desktop {
    function run(): void;
}
declare namespace apps {
    class three_app extends Bootstrap {
        readonly appName: string;
        protected init(): void;
    }
}
