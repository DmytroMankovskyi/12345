﻿import React from 'react';
import { Route } from 'react-router';
import Layout from './components/shared/Layout';
import Home from './components/Home';

export default () => (
  <Layout>
        <Route exact path='/home/events/:page' component={Home} />
  </Layout>
);
