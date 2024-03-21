imports "debugger" from "ServiceHub";

let msi_app = debugger::load_session("\\192.168.1.254\backup3\项目以外内容\客户测试项目交付\mzkit_test\union\union.mzPack");
let layers = debugger::load_summary_layer(msi_app);