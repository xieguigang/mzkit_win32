imports "RedisApp" from "MZWorkRedis";

#' In-memory redis database services for host mzpack objects
#'

[@info "the tcp port for run debugging in VisualStudio."]
[@type "integer"]
const debugPort as string = ?"--port"   || NULL;
[@info "the PID of the master process."]
[@type "integer"]
const master as string    = ?"--master" || NULL;

options(memory.load = "max");

RedisApp::run(as.integer(debugPort));