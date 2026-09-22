// @ts-check
// `@type` JSDoc annotations allow editor autocompletion and type checking
// (when paired with `@ts-check`).
// There are various equivalent ways to declare your Docusaurus config.
// See: https://docusaurus.io/docs/api/docusaurus-config

import {themes as prismThemes} from 'prism-react-renderer';

/**
 * The base url of the deployed site. Can be overridden by an environment
 * variable, so that a fork or a preview deployment doesn't need a code change.
 */
const siteUrl = process.env.DOCS_URL ?? 'https://docs.munique.net';
const baseUrl = process.env.DOCS_BASE_URL ?? '/';

const githubRepo = 'https://github.com/MUnique/OpenMU';

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'OpenMU',
  tagline: 'An easy to use, extendable and customizable MU Online server',
  // Placeholder icon - replace with a real OpenMU logo when there is one.
  favicon: 'img/favicon.svg',

  url: siteUrl,
  baseUrl,

  organizationName: 'MUnique',
  projectName: 'OpenMU',

  // A broken link is a bug, and it should fail the build instead of shipping a
  // dead link to the readers.
  onBrokenLinks: 'throw',
  onBrokenAnchors: 'throw',

  markdown: {
    hooks: {
      onBrokenMarkdownLinks: 'throw',
      onBrokenMarkdownImages: 'throw',
    },
  },

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  future: {
    v4: true,
    faster: true,
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          routeBasePath: '/',
          sidebarPath: './sidebars.js',
          editUrl: `${githubRepo}/tree/master/docs-website/`,
          showLastUpdateTime: true,
        },
        blog: false,
        theme: {
          customCss: './src/css/custom.css',
        },
      }),
    ],
  ],

  themes: [
    [
      // Offline/local search: no external service, no account, works in a fork.
      '@easyops-cn/docusaurus-search-local',
      /** @type {import('@easyops-cn/docusaurus-search-local').PluginOptions} */
      ({
        hashed: true,
        indexBlog: false,
        docsRouteBasePath: '/',
highlightSearchTermsOnTargetPage: true,
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      colorMode: {
        respectPrefersColorScheme: true,
      },
      navbar: {
        title: 'OpenMU',
        logo: {
          alt: 'OpenMU',
          src: 'img/favicon.svg',
        },
        items: [
          {
            type: 'docSidebar',
            sidebarId: 'docsSidebar',
            position: 'left',
            label: 'Documentation',
          },
          {
            to: '/admin-panel/overview',
            position: 'left',
            label: 'Admin Panel',
          },
          {
            href: 'https://discord.gg/2u5Agkd',
            label: 'Discord',
            position: 'right',
          },
          {
            href: githubRepo,
            label: 'GitHub',
            position: 'right',
          },
        ],
      },
      footer: {
        style: 'dark',
        links: [
          {
            title: 'Documentation',
            items: [
              {label: 'Getting Started', to: '/getting-started/requirements'},
              {label: 'Deployment', to: '/deployment/overview'},
              {label: 'Admin Panel', to: '/admin-panel/overview'},
              {label: 'Development', to: '/development/architecture'},
            ],
          },
          {
            title: 'Community',
            items: [
              {label: 'Discord', href: 'https://discord.gg/2u5Agkd'},
              {label: 'Blog', href: 'https://munique.net'},
            ],
          },
          {
            title: 'More',
            items: [
              {label: 'GitHub', href: githubRepo},
              {label: 'Issues', href: `${githubRepo}/issues`},
              {
                label: 'License (MIT)',
                href: `${githubRepo}/blob/master/LICENSE`,
              },
            ],
          },
        ],
        copyright: `Copyright © ${new Date().getFullYear()} MUnique. Built with Docusaurus.`,
      },
      prism: {
        theme: prismThemes.github,
        darkTheme: prismThemes.dracula,
        additionalLanguages: ['csharp', 'bash', 'json', 'yaml', 'nginx', 'powershell'],
      },
    }),
};

export default config;
