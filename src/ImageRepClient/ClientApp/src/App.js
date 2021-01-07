import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import Home from './pages/Home';
import EditImage from './pages/EditImage';
import AddImage from './pages/AddImage';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
            <Route exact path='/' component={Home} />
            <Route path='/edit/:id' component={EditImage} />
            <Route path='/add' component={AddImage} />
      </Layout>
    );
  }
}

