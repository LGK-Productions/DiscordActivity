import { patchUrlMappings } from "./node_modules/@discord/embedded-app-sdk/output/utils/patchUrlMappings";

patchUrlMappings([{
  prefix: "/api",
  target: "discord.lgk-productions.com"
}]);
