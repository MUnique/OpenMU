// @ts-check

/**
 * The sidebar is maintained by hand, so the reading order is a deliberate
 * decision and not a side effect of the file names.
 *
 * @type {import('@docusaurus/plugin-content-docs').SidebarsConfig}
 */
const sidebars = {
  docsSidebar: [
    'intro',
    {
      type: 'category',
      label: 'Getting Started',
      collapsed: false,
      items: [
        'getting-started/requirements',
        'getting-started/docker',
        'getting-started/from-source',
        'getting-started/game-client',
        'getting-started/test-accounts',
      ],
    },
    {
      type: 'category',
      label: 'Deployment',
      items: [
        'deployment/overview',
        'deployment/all-in-one',
        'deployment/all-in-one-traefik',
        'deployment/distributed',
        'deployment/startup-parameters',
      ],
    },
    {
      type: 'category',
      label: 'Admin Panel',
      items: [
        'admin-panel/overview',
        'admin-panel/setup',
        'admin-panel/configuration-updates',
        'admin-panel/servers',
        'admin-panel/accounts',
        'admin-panel/online-accounts',
        'admin-panel/game-configuration',
        'admin-panel/plugins',
        'admin-panel/chat-commands',
        'admin-panel/map-editor',
        'admin-panel/live-map',
        'admin-panel/logs-and-monitoring',
        'admin-panel/authentication',
        'admin-panel/users',
        'admin-panel/common-tasks',
      ],
    },
    {
      type: 'category',
      label: 'Server features',
      items: [
        'server-features/bots',
      ],
    },
    {
      type: 'category',
      label: 'Development',
      items: [
        'development/architecture',
        'development/solution-structure',
        'development/contributing',
      ],
    },
    {
      type: 'category',
      label: 'Reference',
      items: [
        'reference/ports',
        'reference/packets',
      ],
    },
  ],
};

export default sidebars;
